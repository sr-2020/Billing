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
        bool AddInsurance(int modelId);
        InsuranceDto GetInsurance(int modelId);
    }

    public class InsuranceManager : BillingManager, IInsuranceManager
    {
        public InsuranceDto GetInsurance(int modelId)
        {
            var dbinsurance = Get<ProductType>(p => p.Alias == ProductTypeEnum.Insurance.ToString());
            if (dbinsurance == null)
                throw new Exception("insurance type not found");
            var sin = GetSINByModelId(modelId, s => s.Passport);
            if (sin == null)
                throw new Exception("sin not found");
            var insurance = new InsuranceDto
            {
                BuyTime = DateTime.MinValue,
                SkuName = "Страховка отсутствует",
                LifeStyle = "Страховка отсутствует",
                ShopName = "Страховка отсутствует",
                PersonName = $"{sin.Passport?.PersonName}"
            };
            var lastIns = GetInsurance(modelId, r => r.Sku.Nomenklatura, r => r.Shop);
            if (lastIns != null)
            {
                insurance.SkuId = lastIns.SkuId;
                insurance.BuyTime = lastIns.DateCreated;
                insurance.SkuName = lastIns.Sku.Name;
                insurance.LifeStyle = BillingHelper.GetLifestyle(lastIns.Sku.Nomenklatura.Lifestyle).ToString();
                insurance.ShopName = lastIns.Shop.Name;
            }
            return insurance;
        }

        public void AddInsurances()
        {
            var allsins = GetActiveSins(s => s.Character);
            var suc = 0;
            var err = 0;
            foreach (var sin in allsins)
            {
                try
                {
                    if (AddInsurance(sin.Character.Model))
                    {
                        suc++;
                    }
                    else
                    {
                        err++;
                    }
                    for (int i = 0; i < 250; i++)
                    {
                        AddFood(sin.Character.Model);
                    }
                }
                catch (Exception e)
                {
                    err++;
                }
            }
        }

        public bool AddFood(int modelId)
        {
            var shop = 3;
            var sku = 662;
            return AddItem(modelId, sku, shop);
        }

        public bool AddInsurance(int modelId)
        {
            var shopId = 3;
            var skuId = 663;
            return AddItem(modelId, skuId, shopId);
        }

        public bool AddItem(int modelId, int skuId, int shopId)
        {
            var price = GetPrice(modelId, shopId, skuId);
            var renta = ConfirmRenta(modelId, price.PriceId);
            return renta.RentaId > 0;
        }

    }
}
