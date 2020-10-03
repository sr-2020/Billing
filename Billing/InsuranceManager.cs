using Billing.Dto;
using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing
{
    public interface IInsuranceManager : IBaseBillingRepository
    {
        void AddInsurances();
        bool AddInsurance(int modelId, int skuId, int shopId);
        InsuranceDto GetInsurance(int modelId);
    }

    public class InsuranceManager : BillingManager, IInsuranceManager
    {
        public InsuranceDto GetInsurance(int modelId)
        {
            var insuranceId = _settings.GetIntValue(Core.Primitives.SystemSettingsEnum.insuranceid);
            var sin = GetSINByModelId(modelId);
            if (sin == null)
                throw new Exception("sin not found");
            var lastIns = GetList<Renta>(r => r.Sku.Nomenklatura.ProductTypeId == insuranceId && r.SinId == sin.Id, r => r.Shop, r => r.Sku.Nomenklatura)
                                .OrderByDescending(r => r.DateCreated)
                                .FirstOrDefault();
            var insurance = new InsuranceDto
            {
                BuyTime = lastIns.DateCreated,
                SkuName = lastIns.Sku.Name,
                LifeStyle = BillingHelper.GetLifestyle(lastIns.Sku.Nomenklatura.Lifestyle).ToString(),
                ShopName = lastIns.Shop.Name,
                PersonName = sin.PersonName
            };
            return insurance;
        }

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
