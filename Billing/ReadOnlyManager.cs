using Billing.Dto;
using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing
{
    public interface IReadOnlyManager : IBaseBillingRepository
    {
        List<CorporationSkuSoldDto> GetSkuSolds(int beatId);
        List<AnonDto> GetAnons();
        List<InsolventDto> GetInsolvents();
    }
    public class ReadOnlyManager : BaseBillingRepository, IReadOnlyManager
    {
        public List<CorporationSkuSoldDto> GetSkuSolds(int beatId)
        {
            var allRents = GetList<Renta>(r => r.BeatId == beatId, r => r.Sku.Nomenklatura.Specialisation);
            var corpIds = allRents.Select(c => c.Sku.CorporationId);
            var corporations = GetList<CorporationWallet>(c => corpIds.Contains(c.Id));
            var result = new List<CorporationSkuSoldDto>();
            foreach (var corporation in corporations)
            {
                var corpskusolddto = new CorporationSkuSoldDto
                {
                    CorporationId = corporation.Id,
                    CorporationName = corporation.Name
                };
                var corprents = allRents.Where(r => r.Sku.CorporationId == corporation.Id);
                corpskusolddto.SumCorpSkuSold = corprents.Sum(s => s.ShopPrice);
                var groupped1 = corprents.GroupBy(g => g.Sku);
                corpskusolddto.Specialisations = new List<SpecialisationSkuSold>();
                foreach (var rentagroup in groupped1)
                {
                    var skusolddto = new SpecialisationSkuSold
                    {
                        Count = rentagroup.Count(),
                        SpecialisationName = rentagroup.Key.Nomenklatura.Specialisation.Name,
                        NomenklaturaName = rentagroup.Key.Nomenklatura.Name,
                        SkuName = rentagroup.Key.Name,
                        ShopPriceSum = rentagroup.Sum(r => r.ShopPrice),
                        HackedPriceSum = rentagroup.Where(h => h.SinId == null).Sum(h => h.ShopPrice)
                    };
                    corpskusolddto.Specialisations.Add(skusolddto);
                }
                result.Add(corpskusolddto);
            }
            return result;
        }
        public List<AnonDto> GetAnons()
        {
            var list = GetList<Transfer>(t => t.Anonymous, t=>t.WalletFrom, t=>t.WalletTo);
            var sins = GetList<SIN>(s => true);
            var shops = GetList<ShopWallet>(s => true);
            
            return list.Select(t => new AnonDto { Amount = t.Amount, From = GetWalletName(t.WalletFrom, false, sins, shops), OperationTime = t.OperationTime, To = GetWalletName(t.WalletTo, false, sins, shops)}).ToList();
        }

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
                    PersonName = sin.Passport.PersonName,
                    IsIrridium = sin.Wallet.IsIrridium
                };
                insolvent.SumOverdraft = personOverdrafts.Where(t => t.WalletFromId == sin.WalletId).Sum(s => s.Amount);
                insolvent.SumRents = rents.Where(r=>r.SinId == sin.Id).Sum(r => BillingHelper.GetFinalPrice(r));
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
