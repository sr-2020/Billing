﻿using Billing.DTO;
using Core;
using Core.Exceptions;
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
        SIN CreateOrUpdatePhysicalWallet(int modelId, decimal balance = 1);
        SIN InitCharacter(int modelId);
        SIN GetSINByModelId(int modelId, params Expression<Func<SIN, object>>[] includes);
        SIN GetSINBySinText(string sinText, params Expression<Func<SIN, object>>[] includes);
        SIN BillingBlocked(int modelId, params Expression<Func<SIN, object>>[] includes);
        SIN BillingBlocked(string sinText, params Expression<Func<SIN, object>>[] includes);
    }

    public class BaseBillingRepository : BaseEntityRepository, IBaseBillingRepository
    {
        protected ISettingsManager _settings = IocContainer.Get<ISettingsManager>();
        private string BlockErrorMessage = $"В данный момент ведется пересчет рентных платежей, попробуйте повторить чуть позже";

        public SIN InitCharacter(int modelId)
        {
            var settings = IoC.IocContainer.Get<ISettingsManager>();
            var defaultbalance = settings.GetDecimalValue(SystemSettingsEnum.defaultbalance);
            return CreateOrUpdatePhysicalWallet(modelId, defaultbalance);
        }

        public SIN CreateOrUpdatePhysicalWallet(int modelId, decimal balance = 1)
        {
            if (modelId == 0)
                throw new BillingNotFoundException($"character {modelId} not found");
            var character = GetAsNoTracking<Character>(c => c.Model == modelId);
            if (character == null)
                throw new BillingNotFoundException($"character {modelId} not found");
            var sin = Get<SIN>(s => s.Character.Model == modelId);
            if (sin == null)
            {
                sin = new SIN
                {
                    CharacterId = character.Id,
                };
                AddAndSave(sin);
            }
            sin.InGame = true;
            sin.OldMetaTypeId = null;
            sin.EVersion = _settings.GetValue(SystemSettingsEnum.eversion);
            var wallet = CreateOrUpdateWallet(WalletTypes.Character, sin.WalletId, balance);
            sin.Wallet = wallet;
            var scoring = Get<Scoring>(s => s.Id == sin.ScoringId);
            if (scoring == null)
            {
                scoring = new Scoring();
                sin.Scoring = scoring;
                Add(scoring);
            }
            scoring.StartFactor = 0.5m;
            scoring.CurrentFix = 0.5m * 0.5m;
            scoring.CurerentRelative = 0.5m * 0.5m;
            SaveContext();
            InitScoring(scoring);
            if (sin.PassportId == 0)
            {
                var passport = new Passport();
                sin.Passport = passport;
                AddAndSave(passport);
            }
            return sin;
        }

        protected void InitScoring(Scoring scoring)
        {
            var categories = GetList<ScoringCategory>(c => c.CategoryType > 0);
            foreach (var category in categories)
            {
                var current = GetAsNoTracking<CurrentCategory>(c => c.CategoryId == category.Id && c.ScoringId == scoring.Id);
                if (current == null)
                {
                    current = new CurrentCategory
                    {
                        CategoryId = category.Id,
                        ScoringId = scoring.Id,
                        Value = scoring.StartFactor ?? 1
                    };
                    Add(current);
                }
            }
            SaveContext();
        }

        protected string GetWalletName(Wallet wallet, bool anon)
        {
            if (anon)
            {
                return "Anonymous";
            }
            if (wallet == null)
                return string.Empty;
            switch (wallet.WalletType)
            {
                case (int)WalletTypes.Character:
                    var sin = GetAsNoTracking<SIN>(s => s.WalletId == wallet.Id, s => s.Character, s => s.Passport);
                    if (sin == null)
                        return string.Empty;
                    return $"{sin.Character.Model} {sin.Passport.PersonName} {sin.Passport.Sin}";
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

        /// <summary>
        /// TODO need caching
        /// </summary>
        protected TransferDto CreateTransferDto(Transfer transfer, TransferType type, int modelId = 0, string owner = "владелец кошелька")
        {
            bool anon = transfer.Anonymous;
            return new TransferDto
            {
                ModelId = modelId.ToString(),
                Comment = transfer.Comment,
                TransferType = type.ToString(),
                Amount = BillingHelper.Round(transfer.Amount),
                NewBalance = type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime,
                From = type == TransferType.Incoming ? GetWalletName(transfer.WalletFrom, anon) : owner,
                To = type == TransferType.Incoming ? owner : GetWalletName(transfer.WalletTo, anon),
                Anonimous = transfer.Anonymous,
                Id = transfer.Id,
                Overdraft = transfer.Overdraft,
                RentaId = transfer.RentaId
            };
        }

        protected Transfer AddNewTransfer(Wallet walletFrom, Wallet walletTo, decimal amount, string comment, bool anonymous = false, int? rentaId = null, bool overdraft = false)
        {
            if (walletFrom == null)
                throw new BillingNotFoundException($"Нет кошелька отправителя");
            if (walletTo == null)
                throw new BillingNotFoundException($"Нет кошелька получателя");
            if (walletFrom.Id == walletTo.Id)
                throw new BillingException($"Самому себе нельзя переводить.");
            //баланса хватает, или один из кошельков MIR
            if (walletFrom.Balance < amount && walletFrom.WalletType != (int)WalletTypes.MIR && walletTo.WalletType != (int)WalletTypes.MIR)
                throw new BillingException($"Денег нет, но вы держитесь");
            if (amount < 0)
                throw new BillingException($"Нельзя перевести отрицательное значение");
            walletFrom.Balance -= amount;
            walletTo.Balance += amount;
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
                RentaId = rentaId,
                Overdraft = overdraft
            };
            Add(transfer);
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
                    throw new BillingNotFoundException($"кошелек {id} type {type} не найден");
            }
            else
            {
                wallet = new Wallet();
                wallet.WalletType = (int)type;
                Add(wallet);
            }
            wallet.Balance = amount;
            Context.SaveChanges();
            return wallet;
        }

        protected Wallet GetMIR()
        {
            var mir = Get<Wallet>(w => w.Id == GetMIRId() && w.WalletType == (int)WalletTypes.MIR);
            if (mir == null)
                throw new BillingNotFoundException("MIR not found");
            return mir;
        }

        public SIN BillingBlocked(int modelId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = GetSINByModelId(modelId, includes);
            if (sin?.Blocked ?? true)
            {
                throw new BillingException(BlockErrorMessage);
            }
            return sin;
        }

        public SIN BillingBlocked(string sinText, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = GetSINBySinText(sinText);
            if (sin?.Blocked ?? true)
            {
                throw new BillingException(BlockErrorMessage);
            }
            return sin;
        }

        public SIN GetSINByModelId(int modelId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.Character.Model == modelId, includes);
            if (sin == null)
            {
                throw new BillingNotFoundException($"sin for modelId {modelId} not found");
            }
            return sin;
        }

        public SIN GetSINBySinText(string sinText, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.Passport.Sin == sinText, includes);
            if (sin == null)
            {
                throw new BillingNotFoundException($"sin for sinText {sinText} not found");
            }
            return sin;
        }

        protected Sku SkuAllowed(int shop, int sku, params Expression<Func<Sku, object>>[] includes)
        {
            var skuList = GetSkuList(shop, includes);
            return skuList.FirstOrDefault(s => s.Id == sku);
        }

        protected List<Sku> GetSkuList(int shopId, params Expression<Func<Sku, object>>[] includes)
        {
            var skuids = ExecuteQuery<int>($"SELECT * FROM get_sku({shopId})");
            var result = GetList(s => skuids.Contains(s.Id), includes);
            //TODO filter by contractlimit
            return result;
        }

        private int GetMIRId()
        {
            return _settings.GetIntValue(SystemSettingsEnum.MIR_ID);
        }

    }
}
