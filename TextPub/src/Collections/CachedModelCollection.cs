using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Hosting;
using TextPub.Models;

namespace TextPub.Collections
{
    internal abstract class CachedModelCollection<T> : IModelCollection<T> where T : class, IIdentity
    {
        private static MemoryCache _cache = new MemoryCache("TextPub");

        private readonly string _cacheKey = "__TextPub_" + typeof(T).Name + "Collection";

        protected readonly string _filesPath;

        public CachedModelCollection(string filesPath)
        {
            _filesPath = filesPath;
        }
        
        protected IList<T> GetCollection()
        {
            var list = _cache.Get(_cacheKey) as IList<T>;
            
            if (list == null)
            {
                RebuildCache();
            }
            return (IList<T>)_cache.Get(_cacheKey);
        }

        protected void InsertIntoCache(IList<T> collection)
        {
            _cache.Set(_cacheKey, collection, new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
            });
        }

        internal void ClearCache()
        {
            _cache.Remove(_cacheKey);
        }

        protected virtual void RebuildCache()
        {
            var collection = ReadFilesRecursively(_filesPath).ToList();
            InsertIntoCache(collection);
        }

        protected IEnumerable<T> ReadFilesRecursively(string path)
        {
            if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                foreach (var fileInfo in dirInfo.GetFiles())
                {
                    yield return CreateModel(fileInfo, path);
                }

                foreach (var subDirInfo in dirInfo.GetDirectories())
                {
                    foreach (T model in ReadFilesRecursively(path + Path.DirectorySeparatorChar + subDirInfo.Name))
                    {
                        yield return model;
                    }
                }
            }
        }

        protected string GenerateLocalPath(string relativePath, string fileName)
        {
            string path = relativePath.Substring(_filesPath.Length).Replace('\\', Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + fileName;
            return path.TrimStart(Path.DirectorySeparatorChar);
        }

        protected string GenerateId(FileInfo fileInfo, string relativePath)
        {
            string fileName = fileInfo.Name;
            string fileWOExtension = fileName.Substring(0, fileName.Length - fileInfo.Extension.Length);

            string[] pathParts = relativePath.Substring(_filesPath.Length).Replace('\\', '/').Split('/');
            for (int i = 0; i < pathParts.Length; i++)
            {
                pathParts[i] = pathParts[i].UrlFriendly();
            }

            string id = string.Join("/", pathParts) + "/" + fileWOExtension.UrlFriendly();
            return id.TrimStart('/').ToLower();
        }

        protected abstract T CreateModel(FileInfo fileInfo, string relativePath);

        public T this[string id]
        {
            get {
                var collection = GetCollection();
                if (collection == null)
                {
                    return null;
                }

                return collection.SingleOrDefault(m => m.Id == id);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetCollection().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}