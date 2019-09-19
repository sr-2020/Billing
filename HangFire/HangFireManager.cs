using System;

namespace HangFire
{
    public class HangFireManager
    {
        public void DoLongJob()
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            Console.WriteLine(time);
        }

    }
}
