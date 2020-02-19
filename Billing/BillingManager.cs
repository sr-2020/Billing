﻿using Billing.Dto;
using Core;
using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing
{
    //public interface IBillingManager : IDisposable
    //{
    //    #region in the game

    //    Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment);
    //    Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);
    //    Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment);
    //    Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
    //    //TransferDto CreateCredit(int sin, int shop, int owner, decimal amount, string comment);
    //    //SinInfo GetSinInfo(int sin);
    //    //SinDetails GetSinDetails(int sin);
    //    Lifestyles GetLifestyle(int sin);
    //    string GetSinByCharacter(int characterId);
    //    int GetCharacterIdBySin(string sinString);
    //    #endregion
    //    #region info

    //    List<Transfer> GetTransfers(int characterId);

    //    #endregion


    //    #region admin
    //    SINDetails CreatePhysicalWallet(int character, decimal balance);
    //    //create legal wallet

    //    #endregion

    //}

    public class BillingManager : BaseEntityRepository, IDisposable
    {
        private SINDetails GetSINDetailByCharacterId(int characterId, bool includeWallet = false)
        {
            var includes = new List<string>();
            if (includeWallet)
                includes.Add("Wallet");
            if(includes.Any())
                return Get<SINDetails>(s => s.SIN.CharacterId == characterId, includes.ToArray());
            return Get<SINDetails>(s => s.SIN.CharacterId == characterId);
        }

        public List<Transfer> GetTransfers(int characterId)
        {
            var sd = GetSINDetailByCharacterId(characterId);
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

        public SINDetails CreatePhysicalWallet(int character, decimal balance)
        {
            var check = Get<SIN>(s => s.CharacterId == character);
            //var check = new SIN { Id = 2, Character = 1, Citizenship = 1, PersonName = "test2", Sin = "test2", Race = 1 };
            if (check == null)
                throw new Exception("sin not exists");
            var details = Get<SINDetails>(d => d.SINId == check.Id);
            if (details != null)
                throw new Exception("wallet already exists");
            var newWallet = new Wallet()
            {
                Balance = balance,
                Lifestyle = (int)LifeStyleHelper.GetLifeStyle(balance),
                
            };
            Context.Add(newWallet);
            Context.SaveChanges();
            var newDetails = new SINDetails()
            {
                Wallet = newWallet,
                SINId = check.Id
            };
            Context.Add(newDetails);
            Context.SaveChanges();
            return newDetails;
        }

        public Lifestyles GetLifestyle(int sin)
        {
            var details = Get<SINDetails>(s => s.SINId == sin, new string[] { "Wallet" });
            if (details == null)
                throw new Exception("sin not exists");
            return LifeStyleHelper.GetLifeStyle(details.Wallet.Balance);
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
            var d1 = GetSINDetailByCharacterId(characterFrom, true); 
            if (d1 == null)
                throw new Exception($"wallet for {characterFrom} not exists");
            var d2 = GetSINDetailByCharacterId(characterTo, true);
            if (d2 == null)
                throw new Exception($"wallet for {characterTo} not exists");
            return MakeNewTransfer(d1.Wallet, d2.Wallet, amount, comment);
        }

        private Transfer MakeNewTransfer(Wallet wallet1, Wallet wallet2, decimal amount, string comment)
        {
            if (wallet1.Balance < amount)
                throw new Exception($"Need more money on debit wallet {wallet1}");
            Context.Attach(wallet1);
            Context.Attach(wallet2);
            wallet1.Balance -= amount;
            wallet1.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet1.Balance);
            wallet2.Balance += amount;
            wallet2.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet2.Balance);
            Context.SaveChanges();
            var transfer = new Transfer
            {
                Amount = amount,
                Comment = comment,
                WalletFrom = wallet1,
                WalletTo = wallet2,
                NewLifeStyleFrom = wallet1.Lifestyle,
                NewLifeStyleTo = wallet2.Lifestyle
            };
            Context.Add(transfer);
            Context.SaveChanges();
            return transfer;
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
