using Core;
using Core.Model;
using Hangfire;
using Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Jobs
{
    public abstract class BaseJob
    {
        public virtual string JobName { get; }
        public void DoJob()
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                Console.WriteLine($"handle {JobName} started");
                Handle();
                Console.WriteLine($"handle {JobName} finished, elapsed {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                
            }
        }
        public virtual void Handle()
        {
            
        }

    }
}
