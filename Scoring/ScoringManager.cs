using Core;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scoringspace
{
    interface IScoringManager
    {
        void OnLifeStyleChanged(int modelId, Lifestyles from, Lifestyles to);
    }

    public class ScoringManager : BaseEntityRepository, IScoringManager
    {
        public void OnLifeStyleChanged(int sinId, Lifestyles from, Lifestyles to)
        {

            
        }




        public async Task  Change()
        {
            var client = new System.Net.Http.HttpClient();
            await client.GetAsync("");
        }
    }
}
