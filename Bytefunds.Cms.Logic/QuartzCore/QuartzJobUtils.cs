using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.QuartzCore
{
    public class QuartzJobUtils
    {
        private static IScheduler scheduler;
        private static object _lock = new object();
        private static QuartzJobUtils _instance = null;
        public static QuartzJobUtils Instance
        {
            get
            {
                if (_instance == null)
                {

                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            lock (_lock)
                            {
                                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

                                scheduler = schedulerFactory.GetScheduler();
                                _instance = new QuartzJobUtils();
                            }
                        }
                    }
                }
                return _instance;

            }
        }

        public void StartQuartz()
        {
            if (!scheduler.IsStarted)
            {
                scheduler.Start();
                Quartz.IJobDetail job1 = new Quartz.Impl.JobDetailImpl("计算用户最新收益", "EarningsJob", typeof(EarningsJob));
                Quartz.ICronTrigger trigger = new Quartz.Impl.Triggers.CronTriggerImpl("计算用户最新收益触发器", "triggergroup", "计算用户最新收益", "EarningsJob", "0 3 16 * * ?");//每天早上六点执行
                scheduler.ScheduleJob(job1, trigger);
                Common.CustomLog.WriteLog("成功启动任务！" + scheduler.IsStarted);
            }
        }

        public void StopQuartz()
        {
            scheduler.Shutdown(true);
        }
    }
}