using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IInsuranceManager : IBaseBillingRepository
    {
        void AddInsurances();
        bool AddInsurance(int modelId, int skuId, int shopId);
    }

    public class InsuranceManager : BillingManager, IInsuranceManager
    {
        public void AddInsurances()
        {
            var shopId = 3;
            var skuId = 3;
            var allsins = GetList<SIN>(s => true, s => s.Character);
            var suc = 0;
            var err = 0;
            foreach (var sin in allsins)
            {
                try
                {
                    if (AddInsurance(sin.Character.Model, skuId, shopId))
                    {
                        suc++;
                    }
                    else
                    {
                        err++;
                    }
                }
                catch (Exception e)
                {
                    err++;
                }
            }
        }
        public bool AddInsurance(int modelId, int skuId, int shopId)
        {
            var price = GetPrice(modelId, shopId, skuId);
            var renta = ConfirmRenta(modelId, price.PriceId);
            return renta.RentaId > 0;
        }
    }
}
