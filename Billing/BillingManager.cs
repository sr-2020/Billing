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
    public interface IBillingManager
    {
        #region in the game

        Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment);
        Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);
        Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment);
        Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
        string GetSinByCharacter(int characterId);
        int GetCharacterIdBySin(string sinString);

        #endregion
        #region info

        List<TransferDto> GetTransfers(int characterId);
        BalanceDto GetBalance(int characterId);


        #endregion


        #region admin
        SIN CreateOrUpdatePhysicalWallet(int character, decimal balance);
        ProductType CreateOrUpdateProductType(string code, string name, string description);


        //create legal wallet

        #endregion

    }

    public class BillingManager : BaseEntityRepository, IBillingManager
    {
        public BalanceDto GetBalance(int characterId)
        {
            var sin = GetSIN(characterId, s => s.Wallet, s => s.Scoring);
            if (sin == null)
                throw new BillingException("sin not found");
            var balance = new BalanceDto
            {
                CharacterId = characterId,
                CurrentBalance = sin.Wallet.Balance,
                CurrentScoring = sin.Scoring.CurrentScoring,
                SIN = sin.Sin,
                LifeStyle = LifeStyleHelper.GetLifeStyle(sin.Wallet.Balance).ToString()
            };
            return balance;
        }

        public List<TransferDto> GetTransfers(int characterId)
        {
            var sin = GetSIN(characterId);
            if (sin == null)
                throw new BillingException("sin not found");
            var listFrom = GetList<Transfer>(t => t.WalletFromId == sin.WalletId);
            var allList = new List<TransferDto>();
            if (listFrom != null)
                allList.AddRange(listFrom
                    .Select(s => CreateTransferDto(s, TransferType.Outcoming))
                    .ToList());
            var listTo = GetList<Transfer>(t => t.WalletToId == sin.WalletId);
            if (listTo != null)
                allList.AddRange(listTo
                    .Select(s => CreateTransferDto(s, TransferType.Incoming))
                    .ToList());
            return allList.OrderBy(t => t.OperationTime).ToList();
        }

        private TransferDto CreateTransferDto(Transfer transfer, TransferType type)
        {
            return new TransferDto
            {
                Comment = transfer.Comment,
                TransferType = type.ToString(),
                Amount = transfer.Amount,
                NewBalance = type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime
            };
        }


        public string GetSinByCharacter(int characterId)
        {
            var sin = GetSIN(characterId);
            if (sin == null)
                throw new Exception("sin not found");
            return sin.Sin;
        }

        public int GetCharacterIdBySin(string sinString)
        {
            var sin = Get<SIN>(s => s.Sin == sinString);
            if (sin == null)
                throw new Exception("sin not found");
            return sin.CharacterId;
        }

        public ProductType CreateOrUpdateProductType(string code, string name, string description)
        {
            var type = Get<ProductType>(p => p.Code == code);
            if (type == null)
            {
                type = new ProductType();
                type.Code = code;
            }
            Add(type);
            type.Name = name;
            type.Description = description;
            Context.SaveChanges();
            return type;
        }

        public SIN CreateOrUpdatePhysicalWallet(int character, decimal balance)
        {
            var sin = GetSIN(character);

            if (sin == null)
            {
                sin = new SIN
                {
                    CharacterId = character,
                    ScoringId = 0,
                    WalletId = 0
                };
            }
            Add(sin);
            sin.EVersion = IocContainer.Get<ISettingsManager>().GetValue("eversion");

            var wallet = Get<Wallet>(w => w.Id == sin.WalletId);
            if (wallet == null)
            {
                wallet = new Wallet();
                sin.Wallet = wallet;
            }
            Add(wallet);
            wallet.Balance = balance;
            wallet.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(balance);

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

        public Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

        public Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

        public Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

        public Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment)
        {
            var d1 = GetSIN(characterFrom, s => s.Wallet);
            if (d1 == null)
                throw new BillingException($"sin for {characterFrom} not exists");
            var d2 = GetSIN(characterTo, s => s.Wallet);
            if (d2 == null)
                throw new BillingException($"sin for {characterTo} not exists");
            return MakeNewTransfer(d1.Wallet, d2.Wallet, amount, comment);
        }

        #region private

        private SIN GetSIN(int characterId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.CharacterId == characterId, includes);
            return sin;
        }

        private Transfer MakeNewTransfer(Wallet wallet1, Wallet wallet2, decimal amount, string comment)
        {
            if(wallet1 == null)
                throw new BillingException($"wallet1 is null");
            if(wallet2 == null)
                throw new BillingException($"wallet2 is null");
            if (wallet1.Balance < amount)
                throw new BillingException($"Need more money on wallet {wallet1.Id}");
            wallet1.Balance -= amount;
            wallet1.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet1.Balance);
            Add(wallet1);
            wallet2.Balance += amount;
            wallet2.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet2.Balance);
            Add(wallet2);
            Context.SaveChanges();
            var transfer = new Transfer
            {
                Amount = amount,
                Comment = comment,
                WalletFromId = wallet1.Id,
                WalletToId = wallet2.Id,
                NewBalanceFrom = wallet1.Balance,
                NewBalanceTo = wallet2.Balance,
                OperationTime = DateTime.Now
            };
            Add(transfer);
            Context.SaveChanges();
            return transfer;
        }
        #endregion
    }
}
