using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.andrewlinton.music;

namespace Music
{
    public class JobLogRepository
    {
        public JobLog GetLatest()
        {
            JobLog jobLog = null;

            using (var context = new MusicEntities1())
            {
                var jobLogs = from j in context.JobLog
                              orderby j.DateRun
                              select j;

                foreach (var currentJobLog in jobLogs)
                {
                    jobLog = currentJobLog;
                } 
            }

            return jobLog;
        }

        public void Add(string details)
        {
            var jobLog = new JobLog();
            jobLog.DateRun = DateTime.Now;
            jobLog.Details = details;

            using (var context = new MusicEntities1())
            {
                context.JobLog.Add(jobLog);
                context.SaveChanges();
            }
        }
    }
}
