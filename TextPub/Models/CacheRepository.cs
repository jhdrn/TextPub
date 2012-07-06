using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.IO;

namespace TextPub.Models
{
    internal abstract class CacheRepository<T> where T : BaseModel
    {
        private string _cacheKey { get { return typeof(T).Name; } }

        public abstract string RelativeFilesPath { get; }

        public T Get(string id) 
        {
            IList<T> list = GetList();
            if (list == null || !list.Any())
            {
                return null;
            }

            return list.SingleOrDefault(m => m.Id == id);
        }

        public IList<T> GetList()
        {
            var list = (IList<T>)HttpRuntime.Cache.Get(_cacheKey);
            if (list == null)
            {
                RefreshList();
            }
            return (IList<T>)HttpRuntime.Cache.Get(_cacheKey);
        }

        public void PutList(IList<T> list)
        {
            HttpRuntime.Cache.Insert(_cacheKey, list/*, null, DateTime.MaxValue, Cache.NoSlidingExpiration, */);
        }

        public void Remove(string id)
        {
            T item = Get(id);
            if (item == null)
            {
                return;
            }

            IList<T> list = GetList();
            if (list != null)
            {
                list.Remove(item);
            }
        }

        public void RemoveAll()
        {
            HttpRuntime.Cache.Remove(_cacheKey);
        }

        protected virtual void RefreshList()
        {
            IEnumerable<T> list = ReadFilesRecursively(RelativeFilesPath);
            PutList(list.ToList());
        }

        private IEnumerable<T> ReadFilesRecursively(string relativePath)
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
            string path = relativePath.Substring(RelativeFilesPath.Length) + "/" + fileName;
            return path.TrimStart('/', '\\');
        }


        protected string GenerateId(FileInfo fileInfo, string relativePath)
        {
            string fileName = fileInfo.Name;
            string fileWOExtension = fileName.Substring(0, fileName.Length - fileInfo.Extension.Length);

            string[] pathParts = relativePath.Substring(RelativeFilesPath.Length).Replace('\\', '/').Split('/');
            for (int i = 0; i < pathParts.Length; i++)
            {
                pathParts[i] = pathParts[i].UrlFriendly();
            }

            string id = string.Join("/", pathParts) + "/" + fileWOExtension.UrlFriendly();
            return id.TrimStart('/').ToLower();
        }


        protected abstract T CreateModel(FileInfo fileInfo, string relativePath);
    }
}