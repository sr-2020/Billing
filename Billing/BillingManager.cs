using Billing.Dto;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IBillingManager
    {
        #region in the game
        /// <summary>
        /// Прямой перевод между двумя кошельками
        /// </summary>
        /// <param name="walletFrom">ID кошелька отправителя</param>
        /// <param name="walletTo">ID кошелька получателя</param>
        /// <param name="amount">Сумма</param>
        /// <param name="comment">Комментарий, который будет отображаться в истории</param>
        /// <returns></returns>
        //TransferDto MakeTransfer(int walletFrom, int walletTo, decimal amount, string comment);
        /// <summary>
        /// Перевод между двумя физическими лицами
        /// </summary>
        /// <param name="sinFrom">ID паспорта отправителя</param>
        /// <param name="sinTo">ID паспорта получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        //TransferDto MakeTransferSINSIN(int sinFrom, int sinTo, decimal amount, string comment);
        /// <summary>
        /// Перевод с физлица на юрлицо
        /// </summary>
        /// <param name="sinFrom">ID паспорта отправителя</param>
        /// <param name="legTo">ID юрлица получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        //TransferDto MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);
        /// <summary>
        /// Перевод от юрлица физическому лицу
        /// </summary>
        /// <param name="legFrom">ID юрлица отправителя</param>
        /// <param name="sinTo">ID паспорта получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        //TransferDto MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment);
        /// <summary>
        /// Перевод между двумя юрлицами
        /// </summary>
        /// <param name="legFrom">ID юрлица отправителя</param>
        /// <param name="legTo">ID юрлица получателя</param>
        /// <param name="amount">Сумма к переводу</param>
        /// <param name="comment">Комментарий который будет отображаться в истории</param>
        /// <returns></returns>
        //TransferDto MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
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


        #endregion


        #region admin
        //create phisycal wallet
        //create legal wallet

        #endregion

    }

    public class BillingManager : IBillingManager
    {

    }
}
