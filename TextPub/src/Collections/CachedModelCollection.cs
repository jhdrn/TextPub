using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using TextPub.Models;

namespace TextPub.Collections
{

    internal abstract class CachedModelCollection<T> : IModelCollection<T> where T : class, IIdentity
    {
        private string _cacheKey { get { return typeof(T).Name; } }

        protected string _relativeFilesPath { get; private set; }

        public CachedModelCollection(string relativeFilesPath)
        {
            _relativeFilesPath = relativeFilesPath;
        }
        
        protected IList<T> GetCollection()
        {
            var list = (IList<T>)HttpRuntime.Cache.Get(_cacheKey);
            if (list == null)
            {
                RefreshCollection();
            }
            return (IList<T>)HttpRuntime.Cache.Get(_cacheKey);
        }

        protected void PutList(IList<T> list)
        {
            HttpRuntime.Cache.Insert(_cacheKey, list/*, null, DateTime.MaxValue, Cache.NoSlidingExpiration, */);
        }

        internal void ClearCache()
        {
            HttpRuntime.Cache.Remove(_cacheKey);
        }

        protected virtual void RefreshCollection()
        {
            IEnumerable<T> list = ReadFilesRecursively(_relativeFilesPath);
            PutList(list.ToList());
        }

        protected IEnumerable<T> ReadFilesRecursively(string relativePath)
        {
            var absolutePath = HostingEnvironment.MapPath(@"~/App_Data/" + relativePath);

            if (Directory.Exists(absolutePath))
            {
                var dirInfo = new DirectoryInfo(absolutePath);
                foreach (var fileInfo in dirInfo.GetFiles())
                {
                    yield return CreateModel(fileInfo, relativePath);
                }

                foreach (var subDirInfo in dirInfo.GetDirectories())
                {
                    foreach (T model in ReadFilesRecursively(relativePath + Path.DirectorySeparatorChar + subDirInfo.Name))
                    {
                        yield return model;
                    }
                }
            }
        }


        protected string GenerateLocalPath(string relativePath, string fileName)
        {
            string path = relativePath.Substring(_relativeFilesPath.Length) + "/" + fileName;
            return path.TrimStart('/', '\\');
        }


        protected string GenerateId(FileInfo fileInfo, string relativePath)
        {
            string fileName = fileInfo.Name;
            string fileWOExtension = fileName.Substring(0, fileName.Length - fileInfo.Extension.Length);

            string[] pathParts = relativePath.Substring(_relativeFilesPath.Length).Replace('\\', '/').Split('/');
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