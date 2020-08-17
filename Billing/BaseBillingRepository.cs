using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public class BaseBillingRepository : BaseEntityRepository
    {
        public SIN CreateOrUpdatePhysicalWallet(int character = 0, decimal balance = 50)
        {
            if (character == 0)
                throw new BillingAuthException($"character {character} not found");
            var sin = Get<SIN>(s => s.CharacterId == character);
            if (sin == null)
            {
                sin = new SIN
                {
                    CharacterId = character
                };
            }
            Add(sin);
            sin.EVersion = _settings.GetValue(SystemSettingsEnum.eversion);
            var wallet = CreateOrUpdateWallet(WalletTypes.Character, sin.WalletId, balance);
            sin.Wallet = wallet;
            Context.SaveChanges();
            var scoring = Get<Scoring>(s => s.Id == sin.ScoringId);
            if (scoring == null)
            {
                scoring = new Scoring
                {
                    CurrentScoring = 1
                };
                sin.Scoring = scoring;
            }
            Add(scoring);
            Context.SaveChanges();

            //TODO
            var categoryCalculates = GetList<ScoringCategoryCalculate>(c => c.ScoringId == scoring.Id);

            Context.SaveChanges();
            return sin;
        }

        protected ISettingsManager _settings = IocContainer.Get<ISettingsManager>();

        protected string GetWalletName(Wallet wallet)
        {
            if (wallet == null)
                return string.Empty;
            switch (wallet.WalletType)
            {
                case (int)WalletTypes.Character:
                    var sin = Get<SIN>(s => s.WalletId == wallet.Id);
                    if (sin == null)
                        return string.Empty;
                    return $"{sin.CharacterId} {sin.PersonName} {sin.Sin}";
                case (int)WalletTypes.Corporation:
                    var corp = Get<CorporationWallet>(c => c.WalletId == wallet.Id);
                    if (corp == null)
                        return string.Empty;
                    return $"{corp.Id}";
                case (int)WalletTypes.Shop:
                    var shop = Get<ShopWallet>(c => c.WalletId == wallet.Id);
                    if (shop == null)
                        return string.Empty;
                    return $"{shop.Id} {shop.Name}";
                case (int)WalletTypes.MIR:
                    return "MIR";
                default:
                    return string.Empty;
            }
        }

        protected TransferDto CreateTransferDto(Transfer transfer, TransferType type, string owner = "владелец кошелька")
        {
            return new TransferDto
            {
                Comment = transfer.Comment,
                TransferType = type.ToString(),
                Amount = transfer.Amount,
                NewBalance = type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime,
                From = type == TransferType.Incoming ? GetWalletName(transfer.WalletFrom) : owner,
                To = type == TransferType.Incoming ? owner : GetWalletName(transfer.WalletTo),
                Anonimous = transfer.Anonymous
            };
        }

        protected Transfer MakeNewTransfer(Wallet walletFrom, Wallet walletTo, decimal amount, string comment, bool anonymous = false, bool save = true)
        {
            if (walletFrom == null)
                throw new BillingException($"Нет кошелька отправителя");
            if (walletTo == null)
                throw new BillingException($"Нет кошелька получателя");
            if (walletFrom.Id == walletTo.Id)
                throw new BillingException($"Самому себе нельзя переводить.");
            //баланса хватает, или один из кошельков MIR
            if (walletFrom.Balance < amount && walletFrom.WalletType != (int)WalletTypes.MIR && walletTo.WalletType != (int)WalletTypes.MIR)
                throw new BillingException($"Денег нет, но вы держитесь");
            if (amount < 0)
                throw new BillingException($"Нельзя перевести отрицательное значение");
            walletFrom.Balance -= amount;
            Add(walletFrom);
            walletTo.Balance += amount;
            Add(walletTo);
            Context.SaveChanges();
            var transfer = new Transfer
            {
                Amount = amount,
                Comment = comment,
                WalletFromId = walletFrom.Id,
                WalletToId = walletTo.Id,
                NewBalanceFrom = walletFrom.Balance,
                NewBalanceTo = walletTo.Balance,
                OperationTime = DateTime.Now,
                Anonymous = anonymous
            };
            Add(transfer);
            if (save)
                Context.SaveChanges();
            return transfer;
        }

        protected Wallet CreateOrUpdateWallet(WalletTypes type, int id = 0, decimal amount = 0)
        {
            Wallet wallet;
            if (id > 0)
            {
                var inttype = (int)type;
                wallet = Get<Wallet>(w => w.Id == id && w.WalletType == inttype);
                if (wallet == null)
                    throw new Exception($"кошелек {id} type {type} не найден");
            }
            else
            {
                wallet = new Wallet();
                wallet.WalletType = (int)type;
                wallet.Balance = 0;
            }
            Add(wallet);
            Context.SaveChanges();
            if (amount > 0)
            {
                var mir = GetMIR();
                if (wallet.Balance > 0)
                    MakeNewTransfer(wallet, mir, wallet.Balance, "Сброс кошелька");
                wallet.Balance = 0;
                MakeNewTransfer(mir, wallet, amount, "Заведение кошелька");
            }
            return wallet;
        }

        protected Wallet GetMIR()
        {
            var mir = Get<Wallet>(w => w.Id == GetMIRId() && w.WalletType == (int)WalletTypes.MIR);
            if (mir == null)
                throw new Exception("MIR not found");
            return mir;
        }

        protected SIN GetSIN(int characterId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.CharacterId == characterId, includes);
            if (sin == null)
            {
                var defaultBalance = _settings.GetIntValue(SystemSettingsEnum.defaultbalance);
                sin = CreateOrUpdatePhysicalWallet(characterId, defaultBalance);
            }
            return sin;
        }

        protected Sku SkuAllowed(int shop, int sku)
        {
            var skuList = GetSkuList(shop);
            return skuList.FirstOrDefault(s => s.Id == sku);
        }
        protected List<Sku> GetSkuList(int shopId)
        {
            var skuids = ExecuteQuery<int>($"SELECT * FROM get_sku({shopId})");
            var result = GetList<Sku>(s => skuids.Contains(s.Id), s => s.Corporation.Wallet, s => s.Nomenklatura.ProductType);
            //TODO filter by contractlimit
            return result;
        }

        private int GetMIRId()
        {
            return _settings.GetIntValue(SystemSettingsEnum.MIR_ID);
        }

    }
}
