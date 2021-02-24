using Billing.Dto;
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
        List<QRDto> GetAvailableQR(int shop);
        OrganisationViewModel GetAvailableOrganisations(int modelId);
        string GetCharacterName(int modelId);
        List<TransferDto> GetTransfers(int shop);
        Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment);
        Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
        List<RentaDto> GetRentas(int shop);
        void WriteRenta(int rentaId, string qrEncoded);
        int ProcessInflation(decimal k);
    }

    public class ShopManager : AdminManager, IShopManager
    {
        public int ProcessInflation(decimal k)
        {
            var nomenklaturas = GetList<Nomenklatura>(n => true);
            if (nomenklaturas == null)
                throw new Exception("nomenklaturas is null");
            foreach (var nomenklatura in nomenklaturas)
            {
                nomenklatura.BasePrice = nomenklatura.BasePrice * k;
            }
            SaveContext();
            return nomenklaturas.Count;
        }


        public void WriteRenta(int rentaId, string qrEncoded)
        {
            var qr = EreminQrService.GetPayload(qrEncoded);
            var renta = Get<Renta>(p => p.Id == rentaId && p.HasQRWrite && string.IsNullOrEmpty(p.QRRecorded), r => r.Sku.Nomenklatura);
            if (renta == null)
                throw new ShopException($"offer {rentaId} записать на qr невозможно");
            var code = renta.Sku.Nomenklatura.Code;
            var name = renta.Sku.Name;
            var description = renta.Sku.Nomenklatura.Description;
            //TODO
            var count = 1;
            if (!EreminService.WriteQR(qr, code, name, description, count, renta.BasePrice, BillingHelper.GetFinalPrice(renta.BasePrice, renta.Discount, renta.CurrentScoring), renta.Secret, rentaId, (Lifestyles)renta.LifeStyle))
            {
                throw new ShopException("запись на qr не получилось");
            }
            renta.QRRecorded = qr;
            Add(renta);
            Context.SaveChanges();
        }

        public List<RentaDto> GetRentas(int shop)
        {
            var list = GetList<Renta>(r => r.ShopId == shop, r => r.Sku.Nomenklatura.Specialisation.ProductType, r => r.Sku.Corporation, r => r.Shop, r => r.Sin);
            return list.OrderByDescending(r => r.DateCreated)
                    .Select(r =>
                    new RentaDto
                    {
                        FinalPrice = BillingHelper.RoundDown(BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring)),
                        ProductType = r.Sku.Nomenklatura.Specialisation.ProductType.Name,
                        Specialisation = r.Sku.Nomenklatura.Specialisation.Name,
                        Shop = r.Shop.Name,
                        NomenklaturaName = r.Sku.Nomenklatura.Name,
                        SkuName = r.Sku.Name,
                        Corporation = r.Sku.Corporation.Name,
                        HasQRWrite = r.HasQRWrite,
                        QRRecorded = r.QRRecorded,
                        PriceId = r.PriceId,
                        RentaId = r.Id,
                        DateCreated = r.DateCreated,
                        CharacterName = r.Sin.PersonName
                    }).ToList();
        }

        [BillingBlock]
        public Transfer MakeTransferLegLeg(int shopFrom, int shopTo, decimal amount, string comment)
        {
            var shopWalletFrom = Get<ShopWallet>(s => s.Id == shopFrom, s => s.Wallet);
            var shopWalletTo = Get<ShopWallet>(s => s.Id == shopTo, s => s.Wallet);
            var transfer = MakeNewTransfer(shopWalletFrom.Wallet, shopWalletTo.Wallet, amount, comment);
            Context.SaveChanges();
            return transfer;
        }

        [BillingBlock]
        public Transfer MakeTransferLegSIN(int shop, int character, decimal amount, string comment)
        {
            var sin = GetSINByModelId(character, s => s.Wallet);
            var anon = false;
            try
            {
                anon = EreminService.GetAnonimous(character);
            }
            catch (Exception e)
            {

            }
            var shopWallet = Get<ShopWallet>(s => s.Id == shop, s => s.Wallet);
            var transfer = MakeNewTransfer(shopWallet.Wallet, sin.Wallet, amount, comment, anon);
            Context.SaveChanges();
            return transfer;
        }

        public List<TransferDto> GetTransfers(int shop)
        {
            var shopWallet = Get<ShopWallet>(s => s.Id == shop, s => s.Wallet);
            var listFrom = GetList<Transfer>(t => t.WalletFromId == shopWallet.WalletId, t => t.WalletFrom, t => t.WalletTo);

            var allList = new List<TransferDto>();
            var owner = GetWalletName(shopWallet.Wallet, false);
            if (listFrom != null)
                allList.AddRange(listFrom
                    .Select(s => CreateTransferDto(s, TransferType.Outcoming, 0, owner))
                    .ToList());
            var listTo = GetList<Transfer>(t => t.WalletToId == shopWallet.WalletId, t => t.WalletFrom, t => t.WalletTo);
            if (listTo != null)
                allList.AddRange(listTo
                    .Select(s => CreateTransferDto(s, TransferType.Incoming, 0, owner))
                    .ToList());
            return allList.OrderByDescending(t => t.OperationTime).ToList();
        }

        public OrganisationViewModel GetAvailableOrganisations(int modelId)
        {
            var isAdmin = BillingHelper.IsAdmin(modelId);
            var model = new OrganisationViewModel
            {
                CurrentModelId = modelId,
                CurrentCharacterName = GetCharacterName(modelId)
            };
            var sin = GetSINByModelId(modelId);
            model.Shops = GetShops(s => s.OwnerId == sin.Id || isAdmin);
            model.Corporations = GetCorporations(s => s.OwnerId == sin.Id || isAdmin);
            return model;
        }

        public string GetCharacterName(int modelId)
        {
            var character = Get<SIN>(s => s.Character.Model == modelId);
            return character?.PersonName;
        }

        public bool HasAccessToShop(int modelId, int shopId)
        {
            if (modelId == 0)
            {
                throw new BillingUnauthorizedException("Character not authorized");
            }
            var sin = GetSINByModelId(modelId);
            var shop = Get<ShopWallet>(s => s.Id == shopId, s => s.Owner);
            if (shop == null)
            {
                throw new BillingException("shop not found");
            }
            return shop.OwnerId == sin.Id;
        }

        public List<QRDto> GetAvailableQR(int shop)
        {
            var skus = GetSkusForShop(shop);
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



        private List<SkuDto> GetSkusForShop(int shop)
        {
            return GetSkuList(shop).Select(s => new SkuDto(s)).ToList();
        }
        #endregion
    }
}
