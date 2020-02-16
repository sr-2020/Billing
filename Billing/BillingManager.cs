using Billing.Dto;
using Core;
using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IBillingManager
    {
        #region in the game
        /// <summary>
        /// Перевод между двумя физическими лицами
        /// </summary>
        /// <param name="sinFrom">ID паспорта отправителя</param>
        /// <param name="sinTo">ID паспорта получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        Transfer MakeTransferSINSIN(int sinFrom, int sinTo, decimal amount, string comment);
        /// <summary>
        /// Перевод с физлица на юрлицо
        /// </summary>
        /// <param name="sinFrom">ID паспорта отправителя</param>
        /// <param name="legTo">ID юрлица получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);
        /// <summary>
        /// Перевод от юрлица физическому лицу
        /// </summary>
        /// <param name="legFrom">ID юрлица отправителя</param>
        /// <param name="sinTo">ID паспорта получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment);
        /// <summary>
        /// Перевод между двумя юрлицами
        /// </summary>
        /// <param name="legFrom">ID юрлица отправителя</param>
        /// <param name="legTo">ID юрлица получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
        /// <summary>
        /// Создания операции кредит
        /// </summary>
        /// <param name="sinFrom">ID паспорта платильщика</param>
        /// <param name="legTo">ID юрлица бенефициара</param>
        /// <param name="owner">ID юрлица владельца</param>
        /// <param name="amount">Цена предмета</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        //TransferDto CreateCredit(int sin, int shop, int owner, decimal amount, string comment);
        /// <summary>
        /// Получение текущего статуса кошелька sin
        /// </summary>
        /// <param name="sin"></param>
        /// <returns></returns>
        //SinInfo GetSinInfo(int sin);
        /// <summary>
        /// Получение всех операций по sin
        /// </summary>
        /// <param name="sin"></param>
        /// <returns></returns>
        //SinDetails GetSinDetails(int sin);
        Lifestyles GetLifestyle(int sin);

        #endregion


        #region admin
        SINDetails CreatePhysicalWallet(string sin, decimal balance);
        //create legal wallet

        #endregion

    }

    public class BillingManager : BaseEntityRepository, IBillingManager
    {
        public SINDetails CreatePhysicalWallet(string sin, decimal balance)
        {
            var check = Get<SIN>(s => s.Sin == sin);
            //var check = new SIN { Id = 2, Character = 1, Citizenship = 1, PersonName = "test2", Sin = "test2", Race = 1 };
            if (check == null)
                throw new Exception("sin not exists");
            var details = Get<SINDetails>(d => d.SINId == check.Id);
            if (details != null)
                throw new Exception("wallet already exists");
            var newWallet = new Wallet()
            {
                Balance = balance,
                Lifestyle = (int)LifeStyleHelper.GetLifeStyle(balance)
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

        public Transfer MakeTransferSINSIN(int sinFrom, int sinTo, decimal amount, string comment)
        {
            var sin1 = Get<SINDetails>(s => s.SINId == sinFrom, new string[] { "Wallet" });
            if (sin1 == null)
                throw new Exception($"sin {sinFrom} not exists");
            var sin2 = Get<SINDetails>(s => s.SINId == sinTo, new string[] { "Wallet" });
            if (sin2 == null)
                throw new Exception($"sin {sinTo} not exists");
            return MakeNewTransfer(sin1.Wallet, sin2.Wallet, amount, comment);
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

    }
}
