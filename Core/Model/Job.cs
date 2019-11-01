using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Job : BaseEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Cron { get; set; }
        public string JobName { get; set; }
        public JobType JType { get; set; }
        public string HangfireStartId { get; set; }
        public string HangfireReccurentId { get; set; }
        public string HangfireEndId { get; set; }
    }
}
