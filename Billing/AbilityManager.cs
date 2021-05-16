using Core;
using Core.Exceptions;
using Core.Model;
using InternalServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IAbilityManager : IBaseBillingRepository
    {
        void LetHimPay(string modelId, string targetId, string rentaId);
        void LetMePay(string modelId, string rentaId);
        void Rerent(string rentaId);
        void Marauder(string modelId, string targetId);
    }


    public class AbilityManager : AdminManager, IAbilityManager
    {
        public void Marauder(string modelIds, string targetIds)
        {
            var modelId = ParseId(modelIds, "modelId");
            var targetId = ParseId(targetIds, "targetId");
            var sinFrom = BillingBlocked(modelId, s => s.Wallet, s => s.Character, s => s.Passport);
            var sinTo = BillingBlocked(targetId, s => s.Wallet, s => s.Character, s => s.Passport);
            if (!((sinFrom?.Wallet?.Balance ?? 0) > 0))
            {
                EreminPushAdapter.SendNotification(modelId, "Marauder", "у цели недостаточно средств для грабежа");
                return;
            }
            decimal amount = sinFrom.Wallet.Balance * 0.1m;
            var message = $"Ограбление {sinFrom.Passport.Sin} на сумму {amount} в пользу {sinTo.Passport.Sin}";
            MakeTransferSINSIN(sinFrom, sinTo, amount, message);
            EreminPushAdapter.SendNotification(modelId, "Marauder", message);
        }

        public void Rerent(string rentaIds)
        {
            var rentaId = ParseId(rentaIds, "rentaId");
            var renta = Get<Renta>(r => r.Id == rentaId, r => r.Sin.Scoring, r => r.Sin.Character);
            if (renta == null)
            {
                throw new BillingNotFoundException($"renta for {rentaId} not found");
            }
            renta.CurrentScoring = renta.Sin.Scoring.CurerentRelative + renta.Sin.Scoring.CurrentFix;
            Context.SaveChanges();
            EreminPushAdapter.SendNotification(renta.Sin.Character.Model, "Переоформить ренту", "Рента переоформлена");
        }

        public void LetMePay(string modelIds, string rentaIds)
        {
            var modelId = ParseId(modelIds, "modelId");
            var rentaId = ParseId(rentaIds, "rentaId");
            var sin = GetSINByModelId(modelId, s => s.Scoring);
            if (sin == null)
                throw new BillingNotFoundException($"sin for {modelId} not found");

            var renta = Get<Renta>(r => r.Id == rentaId);
            if (renta == null)
            {
                EreminPushAdapter.SendNotification(modelId, "Давай я заплачу", "Ошибка получения ренты");
                throw new BillingNotFoundException($"renta for {rentaIds} not found");
            }
            renta.SinId = sin.Id;
            renta.CurrentScoring = sin.Scoring.CurerentRelative + sin.Scoring.CurrentFix;
            Context.SaveChanges();
            EreminPushAdapter.SendNotification(modelId, "Давай я заплачу", "Рента переоформлена");
        }

        public void LetHimPay(string modelIds, string targetIds, string rentaIds)
        {
            var modelId = ParseId(modelIds, "modelId");
            var targetId = ParseId(targetIds, "targetId");
            var rentaId = ParseId(rentaIds, "rentaId");
            var sin = GetSINByModelId(targetId, s => s.Scoring);
            var renta = Get<Renta>(r => r.Id == rentaId);
            if (renta == null)
            {
                EreminPushAdapter.SendNotification(modelId, "Давай он заплатит", "Ошибка получения ренты");
                throw new BillingNotFoundException($"renta for {rentaIds} not found");
            }
            renta.SinId = sin.Id;
            renta.CurrentScoring = sin.Scoring.CurerentRelative + sin.Scoring.CurrentFix;
            Context.SaveChanges();
            EreminPushAdapter.SendNotification(modelId, "Давай он заплатит", "Рента переоформлена");
        }

        private int ParseId(string id, string field)
        {
            if (!int.TryParse(id, out int intid))
            {
                throw new BillingException($"Ошибка парсинга {field} {id}");
            }
            return intid;
        }
    }
}
