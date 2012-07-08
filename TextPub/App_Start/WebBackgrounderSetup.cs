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

    /// <summary>
    /// Starts and maintains the DropBox sync job.
    /// </summary>
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
            var syncInterval = TextPub.Configuration.DropBoxSyncInterval;

            var jobs = new IJob[]
            {
                new DropBoxSyncJob(TimeSpan.FromMinutes(syncInterval), TimeSpan.FromSeconds(60), TextPub.Configuration)
            };

            var jobHost = new JobHost();

            var manager = new JobManager(jobs, jobHost);
            manager.Fail(e => Elmah.ErrorLog.GetDefault(null).Log(new Error(e)));
            return manager;
        }
    }
}