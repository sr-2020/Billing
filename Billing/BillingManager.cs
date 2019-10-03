using Billing.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IBillingManager
    {
        #region in the game
        TransferResponse MakeTransfer(int walletFrom, int walletTo, decimal amount, string comment);
        TransferResponse MakeTransferSINSIN(int SINFrom, int SINTo, decimal amount, string comment);
        TransferResponse MakeTransferSINShop(int SINFrom, int ShopTo, decimal amount, string comment);
        TransferResponse MakeTransferShopSIN(int ShopFrom, int SINTo, decimal amount, string comment);
        TransferResponse MakeTransferShopShop(int ShopFrom, int ShopTo, decimal amount, string comment);
        //create credit
        //create debit
        //get info
        //get history
        //get detailed info
        #endregion
        //create phisycal wallet
        //create legal wallet

        #region admin


        #endregion

    }

    public class BillingManager : IBillingManager
    {
    }
}
