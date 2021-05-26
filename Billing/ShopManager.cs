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
    public interface IShopManager : IAdminManager
    {
        bool HasAccessToShop(int character, int shop);
        bool HasAccessToCorporation(int character, int corporation);
        List<QRDto> GetAvailableQR(int shop);
        OrganisationViewModel GetAvailableOrganisations(int modelId);
        List<ShopCorporationContractDto> GetCorporationContracts(int corporationId);
        List<ShopCorporationContractDto> GetShopContracts(int shopId);
        void SuggestContract(int corporation, int shop);
        void ApproveContract(int corporation, int shop);
        void ProposeContract(int corporation, int shop);
        void TerminateContract(int corporation, int shop);
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
        EreminService _ereminService = new EreminService();
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
            _ereminService.WriteQR(qr, code, name, description, renta.Count, renta.BasePrice, BillingHelper.GetFinalPrice(renta.BasePrice, renta.Discount, renta.CurrentScoring), renta.Secret, rentaId, (Lifestyles)renta.LifeStyle);
            renta.QRRecorded = qr;
            Add(renta);
            Context.SaveChanges();
        }

        public List<RentaDto> GetRentas(int shop)
        {
            var list = GetList<Renta>(r => r.ShopId == shop, r => r.Sku.Nomenklatura.Specialisation.ProductType, r => r.Sku.Corporation, r => r.Shop, r => r.Sin.Passport);
            return list.OrderByDescending(r => r.DateCreated)
                    .Select(r =>
                    new RentaDto
                    {
                        FinalPrice = BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring),
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
                        CharacterName = r.Sin.Passport.PersonName
                    }).ToList();
        }

        public Transfer MakeTransferLegLeg(int shopFrom, int shopTo, decimal amount, string comment)
        {
            var shopWalletFrom = Get<ShopWallet>(s => s.Id == shopFrom, s => s.Wallet);
            var shopWalletTo = Get<ShopWallet>(s => s.Id == shopTo, s => s.Wallet);
            var transfer = AddNewTransfer(shopWalletFrom.Wallet, shopWalletTo.Wallet, amount, comment);
            Context.SaveChanges();
            return transfer;
        }

        public Transfer MakeTransferLegSIN(int shop, int character, decimal amount, string comment)
        {
            var sin = BillingBlocked(character, s => s.Wallet);
            var anon = false;
            try
            {
                var erService = new EreminService();
                anon = erService.GetAnonimous(character);
            }
            catch (Exception e)
            {

            }
            var shopWallet = Get<ShopWallet>(s => s.Id == shop, s => s.Wallet);
            var transfer = AddNewTransfer(shopWallet.Wallet, sin.Wallet, amount, comment, anon);
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

        public List<ShopCorporationContractDto> GetCorporationContracts(int corporationId)
        {
            var contracts = GetList<Contract>(c => c.CorporationId == corporationId, c => c.Shop).Select(c => new ShopCorporationContractDto(c)).ToList();
            return contracts;
        }
        public List<ShopCorporationContractDto> GetShopContracts(int shopId)
        {
            var contracts = GetList<Contract>(c => c.ShopId == shopId, c => c.Corporation).Select(c => new ShopCorporationContractDto(c)).ToList();
            return contracts;
        }

        public void SuggestContract(int corporation, int shop)
        {
            var contract = Get<Contract>(c => c.CorporationId == corporation && c.ShopId == shop);
            if (contract != null)
            {
                throw new BillingException("Контракт уже создан");
            }
            contract = new Contract { CorporationId = corporation, ShopId = shop, Status = (int)ContractStatusEnum.Suggested };
            AddAndSave(contract);
        }

        public void ApproveContract(int corporation, int shop)
        {
            var statuss = (int)ContractStatusEnum.Suggested;
            var statust = (int)ContractStatusEnum.Terminating;

            var contract = Get<Contract>(c => c.CorporationId == corporation && c.ShopId == shop && (c.Status == statuss || c.Status == statust));
            if (contract == null)
            {
                throw new BillingException("Контракт не найден");
            }
            contract.Status = (int)ContractStatusEnum.Approved;
            AddAndSave(contract);
        }

        public void ProposeContract(int corporation, int shop)
        {
            var statuss = (int)ContractStatusEnum.Suggested;
            var statusa = (int)ContractStatusEnum.Approved;
            var contract = Get<Contract>(c => c.CorporationId == corporation && c.ShopId == shop && (c.Status == statuss || c.Status == statusa));
            if (contract == null)
            {
                throw new BillingException("Контракт не найден");
            }
            if (contract.Status == (int)ContractStatusEnum.Approved)
            {
                contract.Status = (int)ContractStatusEnum.Terminating;
                AddAndSave(contract);
            }
            else
            {
                RemoveAndSave(contract);
            }
        }

        public void TerminateContract(int corporation, int shop)
        {
            var contract = Get<Contract>(c => c.CorporationId == corporation && c.ShopId == shop);
            if (contract == null)
            {
                throw new BillingException("Контракт не найден");
            }
            RemoveAndSave(contract);
        }

        public string GetCharacterName(int modelId)
        {
            var sin = Get<SIN>(s => s.Character.Model == modelId, s=>s.Passport);
            if(sin?.Passport == null)
            {
                return $"Unknown name for {modelId}";
            }
            return sin.Passport.PersonName;
        }

        public bool HasAccessToShop(int modelId, int shopId)
        {
            if (modelId == 0 || shopId == 0)
            {
                return false;
            }
            var sin = GetSINByModelId(modelId);
            var shop = Get<ShopWallet>(s => s.Id == shopId, s => s.Owner);
            if (shop == null)
            {
                throw new BillingException("shop not found");
            }
            return shop.OwnerId == sin.Id;
        }

        public bool HasAccessToCorporation(int modelId, int corporation)
        {
            if (modelId == 0)
            {
                throw new BillingUnauthorizedException("Character not authorized");
            }
            var sin = GetSINByModelId(modelId);
            var corp = Get<CorporationWallet>(s => s.Id == corporation, s => s.Owner);
            if (corp == null)
            {
                throw new BillingException("corporation not found");
            }
            return corp.OwnerId == sin.Id;
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
            return GetSkuList(shop, s => s.Corporation.Wallet, s => s.Nomenklatura.Specialisation.ProductType).Select(s => new SkuDto(s, true)).ToList();
        }
        #endregion
    }
}
