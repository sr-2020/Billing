using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Exceptions;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing
{
    public interface IHackerManager : IAdminManager
    {
        void StealMoney(int modelFrom, int modelTo, decimal amount, string comment);
        void StealShopMoney(int shopId, int modelTo, decimal amount, string comment);
        void StealRenta(int rentaId, int? modelTo);
        void HackShop(int shopId, int[] models);
        ShopDetailedDto GetHackerDetailedShop(int shopId);
        List<ShopDto> GetHackerShops();
        List<CorporationDto> GetHackerCorps();
    }
    public class HackerManager : AdminManager, IHackerManager
    {
        public void HackShop(int shopId, int[] models)
        {
            if (models == null)
            {
                models = new int[0] { };
            }
            UpdateShopTrustedUsers(shopId, models.ToList());
        }

        public void StealMoney(int modelFrom, int modelTo, decimal amount, string comment)
        {
            var from = GetSINByModelId(modelFrom);
            var to = GetSINByModelId(modelTo);
            if (BillingHelper.IsAdmin(modelFrom))
            {
                throw new BillingException("У него нельзя воровать. Ваше местоположение зафиксировано, информация в нужные службы поступила");
            }
            MakeTransferSINSIN(from, to, amount, comment, false);
        }

        public void StealRenta(int rentaId, int? modelTo)
        {
            var renta = Get<Renta>(r => r.Id == rentaId);
            if (renta == null)
                throw new BillingNotFoundException($"renta {rentaId} not found");
            if (!renta.Stealable)
                throw new BillingException("Ренту нельзя украсть");
            if (modelTo == null)
            {
                renta.SinId = null;
            }
            else
            {
                var sin = GetSINByModelId(modelTo ?? 0);
                renta.SinId = sin.Id;
            }
            SaveContext();
        }

        public void StealShopMoney(int shopId, int modelTo, decimal amount, string comment)
        {
            MakeTransferLegSIN(shopId, modelTo, amount, comment);
        }

        public ShopDetailedDto GetHackerDetailedShop(int shopId)
        {
            return GetDetailedShop(shopId);
        }

        public List<ShopDto> GetHackerShops()
        {
            return GetShops(0, s => true);
        }

        public List<CorporationDto> GetHackerCorps()
        {
            return GetCorporationDtos(s => true);
        }
    }
}
