using System;

namespace JobManager
{
    public class JobManager
    {
        public void DoLongJob()
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Console.WriteLine(time);
        }

    }
}
