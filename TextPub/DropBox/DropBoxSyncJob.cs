using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBackgrounder;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using Elmah;
using System.Web.Hosting;
using TextPub.Models;
using TextPub.DropBox.Models;

namespace TextPub.DropBox
{
    /// <summary>
    /// Handles synchronization with DropBox. Checks DropBox's delta to copy/delete/overwrite any new/deleted/modified files.
    /// </summary>
    internal class DropBoxSyncJob : Job
    {
        private DropBoxClient _client;
        private Configuration _configuration;

        public DropBoxSyncJob(TimeSpan interval, TimeSpan timeout, Configuration configuration)
            : base("DropBox Sync Job", interval, timeout)
        {
            _configuration = configuration;
        }

        public override Task Execute()
        {
            // TODO: Remove this dependency
            if (!TextPub.IsConfigured)
            {
                return null;
            }


            if (_client == null)
            {
                _client = new DropBoxClient(
                    _configuration.DropBoxConsumerToken,
                    _configuration.DropBoxConsumerSecret,
                    _configuration.DropBoxUserToken,
                    _configuration.DropBoxUserSecret
                );
            }

            return new Task(() =>
            {
                CheckDelta(_configuration.DropBoxDeltaCursor);

                // TODO: Remove this dependency
                TextPub.ClearCaches();
            });
        }

        private void CheckDelta(string deltaCursor)
        {
            Delta delta = _client.GetDelta(deltaCursor);

            if (delta == null)
            {
                // TODO: Log
                return;
            }

            foreach (var entry in delta.Entries)
            {

                if (entry.MetaData == null)
                {
                    string localPath = HostingEnvironment.MapPath(@"~/App_Data" + entry.Path);

                    // File or folder has been deleted - delete local folder/file
                    if (Directory.Exists(localPath))
                    {
                        Directory.Delete(localPath, true);
                    }
                    else if (File.Exists(localPath))
                    {
                        File.Delete(localPath);
                    }

                }
                else
                {
                    int basePathEndPosition = entry.Path.IndexOf('/', 1) - 1;
                    if (basePathEndPosition < 1)
                    {
                        basePathEndPosition = entry.Path.Length - 1;
                    }
                    string basePath = entry.Path.Substring(1, basePathEndPosition);
                    string localPath;
                    if (basePath == _configuration.ArticlesPath 
                        || basePath == _configuration.PagesPath
                        || basePath == _configuration.SnippetsPath)
                    {
                        localPath = HostingEnvironment.MapPath(@"~/App_Data" + entry.Path);
                    }
                    else
                    {
                        localPath = HostingEnvironment.MapPath(@"~/" + entry.Path);
                    }

                    // Create or delete directory and sync it's contents.
                    if (entry.MetaData.Is_Dir)
                    {
                        if (!Directory.Exists(localPath))
                        {
                            Directory.CreateDirectory(localPath);
                        }


                        continue;
                    }

                    // Compare last modified date time and sync file if DropBox file is newer.
                    var lastLocalWriteTime = File.GetLastWriteTimeUtc(localPath);
                    if (!File.Exists(localPath) || lastLocalWriteTime.CompareTo(entry.MetaData.DateModifiedUTC) < 0)
                    {
                        byte[] fileContents = _client.GetFile(entry.Path);
                        File.WriteAllBytes(localPath, fileContents);
                    }

                }

                _configuration.DropBoxDeltaCursor = delta.Cursor;
                _configuration.Save();

                if (delta.Has_More)
                {
                    CheckDelta(delta.Cursor);
                }
            }
        }
    }
}