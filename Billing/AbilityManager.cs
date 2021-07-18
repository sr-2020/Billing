using Core;
using Core.Exceptions;
using Core.Model;
using InternalServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IAbilityManager : IShopManager
    {
        void LetHimPay(int modelId, int targetId, int rentaId, string qrCode);
        void LetMePay(int modelId, int rentaId, string qrCode);
        void Rerent(int modelId, int rentaId, string qrCode);
        void Marauder(int modelId, int targetId, bool anon = false);
    }

    public class AbilityManager : ShopManager, IAbilityManager
    {
        public void Marauder(int modelId, int targetId, bool anon = false)
        {
            var sinFrom = BillingBlocked(targetId, s => s.Wallet, s => s.Character, s => s.Passport);
            var sinTo = BillingBlocked(modelId, s => s.Wallet, s => s.Character, s => s.Passport);
            if (!((sinFrom?.Wallet?.Balance ?? 0) > 0))
            {
                EreminPushAdapter.SendNotification(modelId, "Marauder", "у цели недостаточно средств для грабежа");
                return;
            }
            decimal amount = sinFrom.Wallet.Balance * 0.1m;
            var message = $"Ограбление {sinFrom.Passport.Sin} на сумму {amount} в пользу {(anon ? "anonymous" : sinTo.Passport.PersonName)}";
            MakeTransferSINSIN(sinFrom, sinTo, amount, message, anon);
            EreminPushAdapter.SendNotification(modelId, "Marauder", message);
        }

        public void Rerent(int modelId, int rentaId, string qrCode)
        {
            var renta = Get<Renta>(r => r.Id == rentaId, r => r.Sin.Character, r => r.Sin.Scoring, r => r.Sin.Passport, r => r.Sku.Nomenklatura);
            if (renta == null)
            {
                ErrorNotify("Переоформить ренту", rentaId, qrCode, modelId);
            }
            RecalculateRenta(renta, qrCode, renta.Sin);
            EreminPushAdapter.SendNotification(modelId, "Переоформить ренту", "Рента переоформлена");
        }

        public void LetMePay(int modelId, int rentaId, string qrCode)
        {
            var sin = GetSINByModelId(modelId, s => s.Scoring, s => s.Character, s => s.Passport);
            if (sin == null)
                throw new BillingNotFoundException($"Син с modelId {modelId} не найден");
            var renta = Get<Renta>(r => r.Id == rentaId, r => r.Sku.Nomenklatura);
            if (renta == null)
            {
                ErrorNotify("Давай я заплачу", rentaId, qrCode, modelId);
            }
            RecalculateRenta(renta, qrCode, sin);
            EreminPushAdapter.SendNotification(modelId, "Давай я заплачу", "Рента переоформлена");
        }

        public void LetHimPay(int modelId, int targetId, int rentaId, string qrCode)
        {
            var sin = GetSINByModelId(targetId, s => s.Scoring, s => s.Character, s => s.Passport);
            var renta = Get<Renta>(r => r.Id == rentaId, r => r.Sku.Nomenklatura);
            if (renta == null)
            {
                ErrorNotify("Давай он заплатит", rentaId, qrCode, modelId);
            }
            RecalculateRenta(renta, qrCode, sin);
            EreminPushAdapter.SendNotification(modelId, "Давай он заплатит", "Рента переоформлена");
        }

        private void ErrorNotify(string subject, int rentaId, string qrCode, int modelId)
        {
            var errormessage = $"Рента {rentaId} записанная на {qrCode} не найдена";
            EreminPushAdapter.SendNotification(modelId, subject, errormessage);
            throw new BillingNotFoundException(errormessage);
        }

    }
}
