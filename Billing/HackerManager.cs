using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public interface IHackerManager : IAdminManager
    {
        void StealMoney(int modelFrom, int modelTo, decimal amount, string comment);
        void StealShopMoney(int shopId, int modelTo, decimal amount, string comment);
        void StealRenta(int rentaId, int? modelTo);
        void HackShop(int shopId, int[] models);
    }
    public class HackerManager: AdminManager, IHackerManager
    {
        

    }
}
