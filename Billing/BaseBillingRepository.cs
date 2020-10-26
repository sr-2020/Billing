using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using IoC;
using Scoringspace;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public interface IBaseBillingRepository : IBaseRepository
    {
        SIN CreateOrUpdatePhysicalWallet(int modelId, string name, int? metarace, decimal balance = 50);
        SIN InitCharacter(int modelId, string name, string metarace);
    }

    public class BaseBillingRepository : BaseEntityRepository, IBaseBillingRepository
    {
        public SIN InitCharacter(int modelId, string name, string metarace)
        {
            var race = GetAsNoTracking<Metatype>(m => m.Alias == metarace);
            return CreateOrUpdatePhysicalWallet(modelId, name, race?.Id);
        }
        

        public SIN CreateOrUpdatePhysicalWallet(int modelId, string name, int? metarace, decimal balance = 50)
        {
            if (modelId == 0)
                throw new BillingAuthException($"character {modelId} not found");
            var character = GetAsNoTracking<Character>(c => c.Model == modelId);
            if (character == null)
                throw new BillingAuthException($"character {modelId} not found");
            var sin = Get<SIN>(s => s.Character.Model == modelId);
            if (sin == null)
            {
                sin = new SIN
                {
                    CharacterId = modelId
                };
                Add(sin);
            }
            sin.PersonName = name;
            sin.MetatypeId = metarace;
            sin.EVersion = _settings.GetValue(SystemSettingsEnum.eversion);
            var wallet = CreateOrUpdateWallet(WalletTypes.Character, sin.WalletId, balance);
            sin.Wallet = wallet;
            var scoring = Get<Scoring>(s => s.Id == sin.ScoringId);
            if (scoring == null)
            {
                scoring = new Scoring
                {
                    CurrentScoring = 1,
                    CurrentFix = 1,
                    CurerentRelative = 1
                };
                sin.Scoring = scoring;
                Add(scoring);
            }
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
                    var sin = GetAsNoTracking<SIN>(s => s.WalletId == wallet.Id, s => s.Character);
                    if (sin == null)
                        return string.Empty;
                    return $"{sin.Character.Model} {sin.PersonName} {sin.Sin}";
                case (int)WalletTypes.Corporation:
                    var corp = GetAsNoTracking<CorporationWallet>(c => c.WalletId == wallet.Id);
                    if (corp == null)
                        return string.Empty;
                    return $"{corp.Id}";
                case (int)WalletTypes.Shop:
                    var shop = GetAsNoTracking<ShopWallet>(c => c.WalletId == wallet.Id);
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
                Amount = BillingHelper.RoundDown(transfer.Amount),
                NewBalance = type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime,
                From = type == TransferType.Incoming ? GetWalletName(transfer.WalletFrom) : owner,
                To = type == TransferType.Incoming ? owner : GetWalletName(transfer.WalletTo),
                Anonimous = transfer.Anonymous
            };
        }

        /// <summary>
        /// Create transfer from walletFrom to walletTo. NOTE: No context saved in method. Need to attach wallets to context manually, and call Context.SaveChanges()
        /// </summary>
        /// <param name="walletFrom"></param>
        /// <param name="walletTo"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="anonymous"></param>
        /// <param name="rentaId"></param>
        /// <returns></returns>
        protected Transfer MakeNewTransfer(Wallet walletFrom, Wallet walletTo, decimal amount, string comment, bool anonymous = false, int? rentaId = null)
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
            CheckLifestyle(walletFrom);
            walletTo.Balance += amount;
            CheckLifestyle(walletTo);
            var transfer = new Transfer
            {
                Amount = amount,
                Comment = comment,
                WalletFromId = walletFrom.Id,
                WalletToId = walletTo.Id,
                NewBalanceFrom = walletFrom.Balance,
                NewBalanceTo = walletTo.Balance,
                OperationTime = DateTime.Now.ToUniversalTime(),
                Anonymous = anonymous,
                RentaId = rentaId
            };
            Add(transfer);
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
                Add(wallet);
            }
            //need to call to get id
            Context.SaveChanges();
            if (amount > 0)
            {
                var mir = GetMIR();
                if (wallet.Balance > 0)
                    MakeNewTransfer(wallet, mir, wallet.Balance, "Сброс кошелька");
                wallet.Balance = 0;
                MakeNewTransfer(mir, wallet, amount, "Заведение кошелька");
            }
            Context.SaveChanges();
            return wallet;
        }

        protected Wallet GetMIR()
        {
            var mir = Get<Wallet>(w => w.Id == GetMIRId() && w.WalletType == (int)WalletTypes.MIR);
            if (mir == null)
                throw new Exception("MIR not found");
            return mir;
        }

        protected SIN GetSINByModelId(int modelId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.Character.Model == modelId, includes);
            if (sin == null)
            {
                var defaultBalance = _settings.GetIntValue(SystemSettingsEnum.defaultbalance);
                sin = CreateOrUpdatePhysicalWallet(modelId, "", null, defaultBalance);
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

        protected string GetJoinCharacterName(int modelId)
        {
            var currentCharacterName = GetAsNoTracking<JoinCharacter>(j => j.Character.Model == modelId, c => c.Character);
            return currentCharacterName?.Name;
        }

        private int GetMIRId()
        {
            return _settings.GetIntValue(SystemSettingsEnum.MIR_ID);
        }

        private void CheckLifestyle(Wallet wallet)
        {
            if (wallet.WalletType != (int)WalletTypes.Character)
            {
                return;
            }
            var newlifestyle = BillingHelper.GetLifeStyleByBalance(wallet.Balance);
            if (wallet.LifeStyle == null)
            {
                wallet.LifeStyle = (int)newlifestyle;
            }
            if (wallet.LifeStyle == (int)newlifestyle)
            {
                return;
            }
            var oldlifestyle = wallet.LifeStyle.Value;
            wallet.LifeStyle = (int)newlifestyle;
            var scoring = IocContainer.Get<ScoringManager>();
            var sin = GetAsNoTracking<SIN>(s => s.WalletId == wallet.Id, s => s.Scoring);
            if (sin == null)
                throw new BillingException("sin not found");
            scoring.OnLifeStyleChanged(sin.Scoring, BillingHelper.GetLifestyle(oldlifestyle), newlifestyle);
        }

    }
}
