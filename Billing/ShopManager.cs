using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Exceptions;
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
        OrganisationViewModel GetAvailableOrganisations(int modelId);
        List<ShopCorporationContractDto> GetCorporationContracts(int corporationId);
        List<ShopCorporationContractDto> GetShopContracts(int shopId);
        void SuggestContract(int corporation, int shop);
        void ApproveContract(int corporation, int shop);
        void ProposeContract(int corporation, int shop);
        void TerminateContract(int corporation, int shop);
        string GetCharacterName(int modelId);
        List<TransferDto> GetTransfers(int shop);
        Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
        List<RentaDto> GetRentas(int shop);
        void WriteRenta(int rentaId, string qrEncoded);
        List<TransferDto> GetCorporationOverdrafts(int corporation);
        List<TransferDto> GetShopOverdrafts(int shop);
    }

    public class ShopManager : AdminManager, IShopManager
    {
        EreminService _ereminService = new EreminService();

        public void WriteRenta(int rentaId, string qrEncoded)
        {
            var qr = EreminQrService.GetPayload(qrEncoded);
            var renta = Get<Renta>(p => p.Id == rentaId && p.HasQRWrite && string.IsNullOrEmpty(p.QRRecorded), r => r.Sku.Nomenklatura.Specialisation.ProductType);
            if (renta == null)
                throw new BillingNotFoundException($"offer {rentaId} записать на qr невозможно");
            WriteRenta(renta, qr);
            SaveContext();
        }

        public List<RentaDto> GetRentas(int shop)
        {
            var list = GetList<Renta>(r => r.ShopId == shop, r => r.Sku.Nomenklatura.Specialisation.ProductType, r => r.Sku.Corporation, r => r.Shop, r => r.Sin.Passport);
            return list.OrderByDescending(r => r.DateCreated)
                    .Select(r =>
                    new RentaDto(r)).ToList();
        }

        public Transfer MakeTransferLegLeg(int shopFrom, int shopTo, decimal amount, string comment)
        {
            var shopWalletFrom = Get<ShopWallet>(s => s.Id == shopFrom, s => s.Wallet);
            var shopWalletTo = Get<ShopWallet>(s => s.Id == shopTo, s => s.Wallet);
            var transfer = AddNewTransfer(shopWalletFrom.Wallet, shopWalletTo.Wallet, amount, comment);
            SaveContext();
            return transfer;
        }

        public List<TransferDto> GetTransfers(int shop)
        {
            var shopWallet = Get<ShopWallet>(s => s.Id == shop, s => s.Wallet);
            var listFrom = GetListAsNoTracking<Transfer>(t => t.WalletFromId == shopWallet.WalletId, t => t.WalletFrom, t => t.WalletTo);
            var listTo = GetListAsNoTracking<Transfer>(t => t.WalletToId == shopWallet.WalletId, t => t.WalletFrom, t => t.WalletTo);
            var owner = $"{shopWallet.Id} {shopWallet.Name}";
            return CreateTransfersDto(listFrom, listTo, owner);
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
            if (sin == null)
            {
                throw new BillingNotFoundException($"sin not found {modelId}");
            }
            model.Shops = GetShops(modelId, s => (isAdmin || s.OwnerId == modelId || s.TrustedUsers.Any(t => t.Model == modelId)));
            if (isAdmin)
                model.Shops.ForEach(s => s.IsOwner = true);
            model.Corporations = GetCorporationDtos(s => s.OwnerId == modelId || isAdmin);
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
                if (contract.Status == (int)ContractStatusEnum.Terminating)
                {
                    contract.Status = (int)ContractStatusEnum.Approved;
                    SaveContext();
                    return;
                }
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
            var sin = Get<SIN>(s => s.Character.Model == modelId, s => s.Passport);
            if (sin?.Passport == null)
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
            if (sin == null)
            {
                throw new BillingNotFoundException("sin not found");
            }
            var shop = Get<ShopWallet>(s => s.Id == shopId, s => s.Owner);
            if (shop == null)
            {
                throw new BillingNotFoundException("shop not found");
            }
            var trusted = Get<ShopTrusted>(t => t.Model == modelId && t.ShopId == shopId);
            return shop.OwnerId == modelId || trusted != null;
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
            return corp.OwnerId == modelId;
        }

        public List<TransferDto> GetCorporationOverdrafts(int corporation)
        {
            var rentIds = GetList<Renta>(r => r.Sku.CorporationId == corporation).Select(r => r.Id).ToList();
            var transfers = GetList<Transfer>(t => t.Overdraft && rentIds.Contains(t.RentaId ?? 0), t=>t.WalletFrom);
            var owner = Get<CorporationWallet>(c => c.Id == corporation);
            var list = CreateTransfersDto(transfers, owner.Name, TransferType.Incoming, true).OrderByDescending(t => t.OperationTime).ToList();
            return list;
        }

        public List<TransferDto> GetShopOverdrafts(int shop)
        {
            var rentIds = GetList<Renta>(r => r.ShopId == shop).Select(r => r.Id).ToList();
            var transfers = GetList<Transfer>(t => t.Overdraft && rentIds.Contains(t.RentaId ?? 0), t => t.WalletFrom, t => t.WalletTo);
            var owner = Get<ShopWallet>(c => c.Id == shop);
            var list = CreateTransfersDto(transfers, owner.Name, TransferType.Incoming, true).OrderByDescending(t => t.OperationTime).ToList();
            return list;
        }

        #region private

        protected void RecalculateRenta(Renta renta, string qrDecoded, SIN newsin)
        {
            renta.SinId = newsin.Id;
            var anon = GetAnon(newsin.Character.Model);
            renta.CurrentScoring = newsin.Scoring.CurerentRelative + newsin.Scoring.CurrentFix;
            var gmdescript = BillingHelper.GetGmDescription(newsin.Passport, renta.Sku, anon);
            _ereminService.UpdateQR(qrDecoded, renta.BasePrice,
                BillingHelper.GetFinalPrice(renta),
                gmdescript,
                renta.Id,
                BillingHelper.GetLifestyle(renta.LifeStyle)).GetAwaiter().GetResult();
            SaveContext();
        }

        private void WriteRenta(Renta renta, string qrDecoded)
        {
            var code = renta.Sku.Nomenklatura.Code;
            var pt = ProductTypeEnum.Spirit.ToString();
            if (renta.Sku.Nomenklatura.Specialisation.ProductType.Alias == pt)
            {
                var magic = new MagicService();
                var intqr = int.Parse(qrDecoded);
                magic.PutSpiritInJar(intqr, code).GetAwaiter().GetResult();
                qrDecoded = $"spirit {qrDecoded}";
            }
            else
            {
                var name = renta.Sku.Name;
                var description = renta.Sku.Nomenklatura.Description;
                _ereminService.WriteQR(qrDecoded, code, name, description, renta.Count, renta.BasePrice, BillingHelper.GetFinalPrice(renta), renta.Secret, renta.Id, (Lifestyles)renta.LifeStyle).GetAwaiter().GetResult();
                var oldQR = Get<Renta>(r => r.QRRecorded == qrDecoded);
                if (oldQR != null)
                    oldQR.QRRecorded = $"{qrDecoded} deleted";
            }
            renta.QRRecorded = qrDecoded;
        }

        #endregion
    }
}
