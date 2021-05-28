using Core;
using Serialization;
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
        void Read(string message);
        void Stop();
    }

    public class PubSubService<T>: IPubSubService
    {
        public PubSubService(string id)
        {
            var env = Environment.GetEnvironmentVariable(SystemHelper.Billing);
            SubscriptionId = $"{env}_{id}";
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
                        _adapter.PullMessages(SubscriptionId, Read);
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    IsRunning = false;
                }
            });
        }

        public virtual void Handle(T model)
        {

        }

        public virtual void Read(string message)
        {
            T model;
            try
            {
                Console.WriteLine($"read {SubscriptionId} message {message}");
                model = Serializer.Deserialize<T>(message);
                Handle(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public virtual void Stop()
        {
            IsRunning = false;
        }

    }
}
