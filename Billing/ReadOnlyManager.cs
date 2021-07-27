using Billing.Dto;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing
{
    public interface IReadOnlyManager : IBaseBillingRepository
    {
        List<InsolventDto> GetInsolvents();
    }
    public class ReadOnlyManager : BaseBillingRepository, IReadOnlyManager
    {
        public List<InsolventDto> GetInsolvents()
        {
            var overdrafts = GetList<Transfer>(t => t.Overdraft == true);
            var listIds = overdrafts.Select(s => s.WalletFromId).Distinct().ToList();
            var sins = GetList<SIN>(s => listIds.Contains(s.WalletId ?? 0), s => s.Wallet, s => s.Passport, s => s.Character);
            var sinIds = sins.Select(s => s.Id).ToList();
            var rents = GetList<Renta>(r => sinIds.Contains(r.SinId ?? 0), r => r.Sku);
            var insolvents = new List<InsolventDto>();
            foreach (var sin in sins)
            {
                var personOverdrafts = overdrafts.Where(o => o.WalletFromId == sin.WalletId);
                var insolvent = new InsolventDto
                {
                    Balance = sin.Wallet.Balance,
                    Citizenship = sin.Passport.Citizenship,
                    ModelId = sin.Character.Model,
                    SinText = sin.Passport.Sin,
                    PersonName = sin.Passport.PersonName
                };
                insolvent.SumOverdraft = personOverdrafts.Where(t => t.WalletFromId == sin.WalletId).Sum(s => s.Amount);
                insolvent.SumRents = rents.Sum(r => BillingHelper.GetFinalPrice(r));
                insolvent.Debts = new List<Debt>();
                foreach (var personaloverdraft in personOverdrafts)
                {
                    var renta = rents.FirstOrDefault(r => r.Id == personaloverdraft.RentaId);
                    var skuName = "взломано";
                    if (renta != null)
                    {
                        skuName = renta.Sku.Name;
                    }
                    var debt = new Debt
                    {
                        FinalPrice = personaloverdraft.Amount,
                        RentId = personaloverdraft.RentaId ?? 0,
                        SkuName = skuName
                    };
                    insolvent.Debts.Add(debt);
                }
                insolvents.Add(insolvent);
            }
            return insolvents;
        }
    }
}
