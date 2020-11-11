using Core;
using Core.Model;
using Hangfire;
using Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jobs
{
    public abstract class BaseJob
    {
        public virtual string JobName { get; }
        public Task DoJob()
        {
            return Task.Run(() =>
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
                    Console.WriteLine(e.ToString());
                }
            });

        }
        public virtual void Handle()
        {
            
        }

    }
}
