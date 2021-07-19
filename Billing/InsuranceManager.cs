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

    }
}
