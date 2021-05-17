using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jobs
{
    public class JobLifeDto
    {
        public JobLifeDto()
        {
            History = new List<BeatHistory>();
        }
        public BillingBeat Beat { get; set; }
        public List<BeatHistory> History { get; set; }

        public void AddHistory(string comment)
        {
            Console.WriteLine(comment);
            var history = new BeatHistory
            {
                BeatId = Beat.Id,
                Comment = comment
            };
            History.Add(history);
        }
    }
}
