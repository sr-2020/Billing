using Billing;
using Core;
using Core.Model;
using Core.Primitives;
using Hangfire;
using IoC;
using NCrontab;
using Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jobs
{
    public interface IJobManager : IBaseRepository
    {
        void ProcessPeriod();
        HangfireJob AddOrUpdateJob(int id, DateTimeOffset? start, DateTimeOffset? end, string cron, string jobname, int jobtype);
        List<HangfireJob> GetAllJobs(bool finished, int jobtype);
    }

    public class JobManager : BaseEntityRepository, IJobManager
    {
        private ISettingsManager _settingManager = IocContainer.Get<ISettingsManager>();
        public HangfireJob AddOrUpdateJob(int id, DateTimeOffset? start, DateTimeOffset? end, string cron, string jobname, int jobtype)
        {
            HangfireJob job = null;
            if (id > 0)
                job = Get<HangfireJob>(h => h.Id == id);
            if (job == null)
            {
                job = new HangfireJob();
            }
            if (start.HasValue && start > DateTimeOffset.Now)
            {
                job.StartTime = start.Value;
            }
            if (end.HasValue && end > DateTimeOffset.Now)
            {
                job.EndTime = end.Value;
            }
            if (SystemHelper.CronParse(cron) != null)
            {
                job.Cron = cron;
            }
            if (!string.IsNullOrEmpty(jobname))
            {
                job.JobName = jobname;
            }
            if (Enum.IsDefined(typeof(JobType), jobtype))
            {
                job.JobType = jobtype;
            }
            Add(job);
            Context.SaveChanges();
            job = CreateOrUpdateStartJob(job);

            Add(job);
            Context.SaveChanges();
            return job;
        }

        public void ProcessPeriod()
        {
            Task.Run(() =>
            {
                var cycle = new BillingCycle
                {
                    StartTime = DateTime.Now
                };
                try
                {
                    var processing = _settingManager.GetBoolValue(Core.Primitives.SystemSettingsEnum.block);
                    if (processing)
                    {
                        Console.Error.WriteLine("Процесс пересчета уже запущен! Попытка повторного запуска");
                        return;
                    }
                    Add(cycle);
                    Context.SaveChanges();
                    //lock
                    _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "true");
                    Console.WriteLine("Биллинг заблокирован");
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    return;
                }
                try
                {
                    var _billingManager = IocContainer.Get<IBillingManager>();
                    _billingManager.ProcessPeriod();
                }
                catch (Exception e)
                {
                    
                    Console.WriteLine("ошибка ProcessPeriod");
                    Console.Error.WriteLine(e.Message);
                }
                finally
                {
                    //unlock
                    Console.WriteLine("PeriodJob processing finish");
                    _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.block, "false");
                    Console.WriteLine("Биллинг разблокирован");
                    var version = _settingManager.GetIntValue(Core.Primitives.SystemSettingsEnum.eversion);
                    version++;
                    _settingManager.SetValue(Core.Primitives.SystemSettingsEnum.eversion, version.ToString());
                }
                cycle.FinishTime = DateTime.Now;
                Context.SaveChanges();
            });
        }

        #region hangfire(obsolete)

        public List<HangfireJob> GetAllJobs(bool finished, int jobtype)
        {
            var filter = DateTimeOffset.Now;
            return GetList<HangfireJob>(j => (j.EndTime > filter || finished) && j.JobType == jobtype).ToList();
        }

        private HangfireJob CreateOrUpdateStopJob(HangfireJob job)
        {
            if (!string.IsNullOrEmpty(job.HangfireEndId))
            {
                RecurringJob.RemoveIfExists(job.HangfireEndId);
            }
            var date = job.EndTime;
            job.HangfireEndId = BackgroundJob.Schedule(() => RecurringJob.RemoveIfExists(job.HangfireRecurringId), date);
            return job;
        }

        private HangfireJob CreateOrUpdateStartJob(HangfireJob job)
        {
            if (job.StartTime < DateTimeOffset.Now)
                return job;
            if (!string.IsNullOrEmpty(job.HangfireStartId))
            {
                RecurringJob.RemoveIfExists(job.HangfireStartId);
            }
            var date = job.StartTime;
            job.HangfireStartId = BackgroundJob.Schedule(() => CreateOrUpdateRecurringJob(job.Id), date);
            return job;
        }

        public void CreateOrUpdateRecurringJob(int dbJobId)
        {
            BaseJob job;
            var dbJob = Get<HangfireJob>(j => j.Id == dbJobId);
            switch ((JobType)dbJob.JobType)
            {
                default:
                    throw new Exception("jobtype not found");
            }
            dbJob = CreateOrUpdateStopJob(dbJob);
            Add(dbJob);
            Context.SaveChanges();
        }

        private HangfireJob CreateOrUpdateRecurringJob(HangfireJob dbJob, BaseJob job)
        {
            if (string.IsNullOrEmpty(dbJob.HangfireRecurringId))
            {
                dbJob.HangfireRecurringId = Guid.NewGuid().ToString();
            }

            RecurringJob.AddOrUpdate(dbJob.HangfireRecurringId, () => job.DoJob(), $"{dbJob.Cron}");
            return dbJob;
        }

        #endregion
    }
}
