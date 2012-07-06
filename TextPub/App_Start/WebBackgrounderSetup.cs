using System;
using Elmah;
using WebBackgrounder;
using System.Web.Configuration;
using System.Configuration;
using System.Web;
using System.Web.Hosting;
using TextPub.Models;
using TextPub.DropBox;

[assembly: WebActivator.PostApplicationStartMethod(typeof(TextPub.App_Start.WebBackgrounderSetup), "Start")]
[assembly: WebActivator.ApplicationShutdownMethod(typeof(TextPub.App_Start.WebBackgrounderSetup), "Shutdown")]

namespace TextPub.App_Start
{
    public static class WebBackgrounderSetup
    {

        static readonly JobManager _jobManager = CreateJobWorkersManager();

        public static void Start()
        {
            _jobManager.Start();
        }

        public static void Shutdown()
        {
            _jobManager.Dispose();
        }

        private static JobManager CreateJobWorkersManager()
        {
            string syncIntervalSetting = WebConfigurationManager.AppSettings[TextPub.DropBoxSyncInterval];

            int syncInterval;
            if (!int.TryParse(syncIntervalSetting, out syncInterval))
            {
                // Sync every hour per default
                syncInterval = 60;
            }

            var jobs = new IJob[]
            {
                new DropBoxSyncJob(TimeSpan.FromMinutes(syncInterval), TimeSpan.FromSeconds(60), TextPub.ArticleRepository, TextPub.PageRepository, TextPub.SnippetRepository)
            };

            var jobHost = new JobHost();

            var manager = new JobManager(jobs, jobHost);
            manager.Fail(e => Elmah.ErrorLog.GetDefault(null).Log(new Error(e)));
            return manager;
        }
    }
}