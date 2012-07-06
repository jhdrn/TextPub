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

namespace TextPub.DropBox
{
    internal class DropBoxSyncJob : Job
    {
        private DropBoxClient _client;

        private ArticleRepository _articleRepository;
        private PageRepository _pageRepository;
        private SnippetRepository _snippetRepository;

        public DropBoxSyncJob(TimeSpan interval, TimeSpan timeout, ArticleRepository articleRepository, PageRepository pageRepository, SnippetRepository snippetRepository)
            : base("DropBox Sync Job", interval, timeout)
        {
            _articleRepository = articleRepository;
            _pageRepository = pageRepository;
            _snippetRepository = snippetRepository;
        }

        public override Task Execute()
        {

            if (!TextPub.IsConfigured)
            {
                return null;
            }


            if (_client == null)
            {
                _client = new DropBoxClient(
                    TextPub.ConfigurationManager.Get(TextPub.DropBoxConsumerToken),
                    TextPub.ConfigurationManager.Get(TextPub.DropBoxConsumerSecret),
                    TextPub.ConfigurationManager.Get(TextPub.DropBoxUserToken),
                    TextPub.ConfigurationManager.Get(TextPub.DropBoxUserSecret)
                );
            }

            return new Task(() =>
            {
                CheckDelta(null, null, TextPub.ConfigurationManager.Get(TextPub.DropBoxDeltaCursor));
            });
        }

        private void CheckDelta(Configuration config, AppSettingsSection appSettings, string deltaCursor)
        {
            Delta delta = _client.GetDelta(deltaCursor);

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
                    if (basePath == _articleRepository.RelativeFilesPath
                        || basePath == _pageRepository.RelativeFilesPath
                        || basePath == _snippetRepository.RelativeFilesPath)
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

                TextPub.ConfigurationManager.Put(TextPub.DropBoxDeltaCursor, delta.Cursor);

                if (delta.Has_More)
                {
                    CheckDelta(config, appSettings, delta.Cursor);
                }
            }
        }
    }
}