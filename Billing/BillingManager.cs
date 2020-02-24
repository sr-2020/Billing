using Billing.Dto;
using Core;
using Core.Model;
using Core.Primitives;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //TransferDto CreateCredit(int sin, int shop, int owner, decimal amount, string comment);
        //SinInfo GetSinInfo(int sin);
        //SinDetails GetSinDetails(int sin);
        string GetSinByCharacter(int characterId);
        int GetCharacterIdBySin(string sinString);
        #endregion
        #region info

        List<Transfer> GetTransfers(int characterId);

        #endregion


        #region admin
        SIN CreateOrUpdatePhysicalWallet(int character, decimal balance);
        //create legal wallet

        #endregion

    }

    public class BillingManager : BaseEntityRepository, IBillingManager
    {
        public BillingManager() : base() { }

        public List<Transfer> GetTransfers(int characterId)
        {
            var sd = Get<SIN>(s => s.CharacterId == characterId);
            if (sd == null)
                throw new Exception("character not found");
            return GetList<Transfer>(t => t.WalletFromId == sd.WalletId || t.WalletToId == sd.WalletId);
        }

        public string GetSinByCharacter(int characterId)
        {
            var sin = Get<SIN>(s => s.CharacterId == characterId);
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

        public SIN CreateOrUpdatePhysicalWallet(int character, decimal balance)
        {
            var sin = Get<SIN>(s => s.CharacterId == character);

            if (sin == null)
            {
                sin = new SIN
                {
                    CharacterId = character,
                    ScoringId = 0,
                    WalletId = 0
                };
                Context.Add(sin);
            }
            sin.EVersion = IocContainer.Get<ISettingsManager>().GetValue("eversion");

            var wallet = Get<Wallet>(w => w.Id == sin.WalletId);
            if (wallet == null)
            {
                wallet = new Wallet();
                sin.Wallet = wallet;
                Context.Add(wallet);
            }
            wallet.Balance = balance;
            wallet.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(balance);

            var scoring = Get<Scoring>(s => s.Id == sin.ScoringId);
            if (scoring == null)
            {
                scoring = new Scoring
                {
                    CurrentScoring = 0
                };
                Context.Add(scoring);
                sin.Scoring = scoring;
            }
            Context.SaveChanges();


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
            var d1 = Get<SIN>(s => s.CharacterId == characterFrom, x => x.Wallet);
            if (d1 == null)
                throw new Exception($"sin for {characterFrom} not exists");
            var d2 = Get<SIN>(s => s.CharacterId == characterTo, x => x.Wallet);
            if (d2 == null)
                throw new Exception($"sin for {characterTo} not exists");
            return MakeNewTransfer(d1.Wallet, d2.Wallet, amount, comment);
        }

        private Transfer MakeNewTransfer(Wallet wallet1, Wallet wallet2, decimal amount, string comment)
        {
            if (wallet1.Balance < amount)
                throw new Exception($"Need more money on wallet {wallet1}");
            wallet1.Balance -= amount;
            wallet1.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet1.Balance);
            Context.Entry(wallet1).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            wallet2.Balance += amount;
            wallet2.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet2.Balance);
            Context.Entry(wallet2).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            Context.SaveChanges();
            var transfer = new Transfer
            {
                Amount = amount,
                Comment = comment,
                WalletFromId = wallet1.Id,
                WalletToId = wallet2.Id,
                NewBalanceFrom = wallet1.Balance,
                NewBalanceTo = wallet2.Balance
            };
            Context.Add(transfer);
            Context.SaveChanges();
            return transfer;
        }

    }
}
