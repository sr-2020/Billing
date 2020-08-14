using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using InternalServices;
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
        List<QRDto> GetAvailableQR(int shop);
        ShopViewModel GetAvailableShops(int character);
        string GetCharacterName(int character);
        List<TransferDto> GetTransfers(int shop);

    }

    public class ShopManager : BaseBillingRepository, IShopManager
    {
        public List<TransferDto> GetTransfers(int shop)
        {
            var shopWallet = Get<ShopWallet>(s => s.Id == shop, s => s.Wallet);
            var listFrom = GetList<Transfer>(t => t.WalletFromId == shopWallet.WalletId, t => t.WalletFrom, t => t.WalletTo);
            
            var allList = new List<TransferDto>();
            var owner = GetWalletName(shopWallet.Wallet);
            if (listFrom != null)
                allList.AddRange(listFrom
                    .Select(s => CreateTransferDto(s, TransferType.Outcoming))
                    .ToList());
            var listTo = GetList<Transfer>(t => t.WalletToId == shopWallet.WalletId, t => t.WalletFrom, t => t.WalletTo);
            if (listTo != null)
                allList.AddRange(listTo
                    .Select(s => CreateTransferDto(s, TransferType.Incoming))
                    .ToList());
            return allList.OrderBy(t => t.OperationTime).ToList();
        }

        public ShopViewModel GetAvailableShops(int character)
        {
            var model = new ShopViewModel
            {
                CurrentCharacterId = character,
                CurrentCharacterName = GetCharacterName(character)
            };
            model.Shops = GetShops(s => s.Owner == character);
            return model;
        }

        public string GetCharacterName(int character)
        {
            var currentCharacterName = Get<JoinCharacter>(j => j.Character.Model == character, c => c.Character);
            return currentCharacterName?.Name;
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
            if (shop == null)
            {
                throw new BillingException("Магазин не найден");
            }
            return shop;
        }

        public List<ShopDto> GetShops(Expression<Func<ShopWallet, bool>> predicate)
        {
            return GetList(predicate, new string[] { "Wallet", "Specialisations", "Specialisations.ProductType" }).Select(s =>
                     new ShopDto()
                     {
                         Id = s.Id,
                         Name = s.Name,
                         OwnerId = s.Owner,
                         Comission = s.Commission,
                         Lifestyle = ((Lifestyles)s.LifeStyle).ToString(),
                         Balance = s.Wallet.Balance,
                         Specialisations = CreateSpecialisationDto(s)
                     }).ToList();
        }

        public List<QRDto> GetAvailableQR(int shop)
        {
            var billing = IocContainer.Get<IBillingManager>();
            var skus = billing.GetSkusForShop(shop);
            var qrs = new List<QRDto>();
            foreach (var sku in skus)
            {
                var qr = CreateQRDto(shop, sku);
                qrs.Add(qr);
            }
            return qrs;
        }

        #region private

        private QRDto CreateQRDto(int shop, SkuDto sku)
        {
            var qr = new QRDto();
            qr.Shop = shop;
            qr.Sku = sku;
            qr.QRID = QRHelper.Concatenate(sku.SkuId, shop);
            var cache = Get<CacheQRContent>(q => q.QRID == qr.QRID);
            if (cache == null)
            {
                cache = new CacheQRContent
                {
                    QRID = qr.QRID,
                    Encoded = EreminQrService.GetQRUrl(qr.QRID)
                };
                Add(cache);
                Context.SaveChanges();
            }
            qr.QR = cache.Encoded;
            return qr;
        }

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
