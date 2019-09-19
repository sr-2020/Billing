using System;

namespace Jobs
{
    public class JobManager
    {
        public JobManager()
        {
            Console.WriteLine("JobManager created");
        }
        public void DoLongJob()
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Console.WriteLine($"DoLongJob succesfully did, {time}");
        }
    }
}
