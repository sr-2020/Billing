using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public interface IShopManager : IBaseRepository
    {
        bool HasAccessToShop(int character, int shop);
        List<ShopDto> GetShops(Expression<Func<ShopWallet, bool>> predicate);
        ShopDto GetShop(int id);
        List<QRDto> GetAvailableSkus(int shop);
        string GetShopName(int shopId);
    }

    public class ShopManager : BaseEntityRepository, IShopManager
    {
        public string GetShopName(int shopId)
        {
            var shop = Get<ShopWallet>(s => s.Id == shopId);
            if (shop == null)
            {
                throw new BillingException("магазин не найден");
            }
            return shop.Name;
        }

        public bool HasAccessToShop(int character, int shopId)
        {
            if (character == 0)
            {
                throw new BillingAuthException("Character not authorized");
            }
            var shop = Get<ShopWallet>(s => s.Id == shopId);
            if (shop == null)
            {
                throw new BillingException("shop not found");
            }
            return shop.Owner == character;
        }

        public ShopDto GetShop(int id)
        {
            var shop = GetShops(s => s.Id == id).FirstOrDefault();
            if(shop == null)
            {
                throw new BillingException("Магазин не найден");
            }
            return shop;
        }

        public List<ShopDto> GetShops(Expression<Func<ShopWallet, bool>> predicate)
        {
            return GetList(predicate, new string[] { "Wallet", "Specialisations", "Specialisations.ProductType" }).Select(s =>
                     new ShopDto(s.Id, s.Name)
                     {
                         Comission = s.Commission,
                         Lifestyle = ((Lifestyles)s.LifeStyle).ToString(),
                         Balance = s.Wallet.Balance,
                         Specialisations = CreateSpecialisationDto(s)
                     }).ToList();
        }

        public List<QRDto> GetAvailableSkus(int shop)
        {
            var billing = IocContainer.Get<IBillingManager>();
            var qr = billing.GetSkusForShop(shop).Select(s=>new QRDto(s, shop));
            return qr.ToList();
        }

        #region private
        private List<SpecialisationDto> CreateSpecialisationDto(ShopWallet shop)
        {
            var list = new List<SpecialisationDto>();
            if (shop.Specialisations == null)
                return list;
            list.AddRange(shop.Specialisations.Select(s => new SpecialisationDto
            {
                ProductTypeId = s.ProductTypeId,
                ProductTypeName = s.ProductType?.Name,
                ShopId = s.ShopId,
                ShopName = shop.Name
            }));
            return list;
        }
        #endregion
    }
}
