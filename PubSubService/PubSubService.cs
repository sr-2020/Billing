using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PubSubService
{
    public interface IPubSubService
    {
        void Run();
        void Handle(string message);
        void Stop();
    }

    public class PubSubService: IPubSubService
    {
        public PubSubService(string id)
        {
            SubscriptionId = id;
            _adapter = new PubSubAdapter();
        }
        private PubSubAdapter _adapter;
        public string SubscriptionId { get; set; }
        public bool IsRunning { get; set; }
        
        public virtual void Run()
        {
            IsRunning = true;
            Task.Run(() =>
            {
                try
                {
                    while (IsRunning)
                    {
                        _adapter.PullMessages(SubscriptionId, Handle);
                        Console.WriteLine("sleep");
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    IsRunning = false;
                }
            });
        }

        public virtual void Handle(string message)
        {
            Console.WriteLine($"message {message} handled");
        }

        public virtual void Stop()
        {
            IsRunning = false;
        }
    }
}
