using System;
using System.Collections.Generic;
using System.Text;

namespace Jobs
{
    public class JobConfig
    {
        public string Name { get; set; }
        public string Instance { get; set; }
        public string CronExpression { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
