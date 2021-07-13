using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Exceptions;
using Core.Model;
using Core.Primitives;
using InternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public interface IAdminManager : IBaseBillingRepository
    {
        Transfer MakeTransferLegSIN(int legFrom, string sinTo, decimal amount, string comment);
        Transfer MakeTransferLegSIN(int shop, int modelId, decimal amount, string comment);
        List<SpecialisationDto> GetSpecialisations(Expression<Func<Specialisation, bool>> predicate);
        List<ShopDto> GetShops(int modelId, Expression<Func<ShopWallet, bool>> predicate);
        ShopDetailedDto GetDetailedShop(int shopId);
        List<CorporationDto> GetCorporationDtos(Expression<Func<CorporationWallet, bool>> predicate);
        CorporationDetailedDto GetCorporationDto(int corporationId);
        List<QRDto> GetAvailableQR(int shop);
        void AddCorpSpecialisation(int corporation, int specialisation, decimal ratio);
        void DeleteCorpSpecialisation(int corporation, int specialisation);
        List<ProductTypeDto> GetProductTypes(Expression<Func<ProductType, bool>> predicate);
        List<NomenklaturaDto> GetNomenklaturas(Expression<Func<Nomenklatura, bool>> predicate);
        List<SkuDto> GetSkus(Expression<Func<Sku, bool>> predicate);
        List<UserDto> GetUsers();
        List<SIN> GetActiveSins(params Expression<Func<SIN, object>>[] includes);
        ShopDto CreateOrUpdateShopWallet(int foreignKey, decimal amount, string name, int lifestyle, int owner, List<int> specialisations, string comment = "", string location = "");
        SpecialisationDto CreateOrUpdateSpecialisation(int id, int producttype, string name);
        NomenklaturaDto CreateOrUpdateNomenklatura(int id, string name, string code, int specialisationId, int lifestyle, decimal baseprice, int baseCount, string description, string pictureurl, int externalId = 0);
        SkuDto CreateOrUpdateSku(int id, int nomenklatura, int count, int corporation, string name, bool enabled, int? skubaseprice = null, int? skubasecount = null);
        CorporationWallet CreateOrUpdateCorporationWallet(int id, decimal amount, string name, string logoUrl);
        void DeleteSpecialisation(int id);
        void DeleteNomenklatura(int id);
        void DeleteShop(int shopid);
        void DeleteSku(int skuId);
        Transfer MakeTransferSINSIN(int characterFrom, string sinTo, decimal amount, string comment);
        Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment);
        void CleanRenta(Renta renta);
        void UpdateShopTrustedUsers(int shopId, List<int> trustedModels);
    }
    public class AdminManager : BaseBillingRepository, IAdminManager
    {
        public static string UrlNotFound = "";
        protected int CURRENTGAME = 1;

        public Transfer MakeTransferLegSIN(int shop, string sintext, decimal amount, string comment)
        {
            var sin = BillingBlocked(sintext, s => s.Wallet, s => s.Character);
            return MakeTransferLegSIN(shop, sin, amount, comment);
        }

        public Transfer MakeTransferLegSIN(int shop, int modelId, decimal amount, string comment)
        {
            var sin = BillingBlocked(modelId, s => s.Wallet, s => s.Character);
            return MakeTransferLegSIN(shop, sin, amount, comment);
        }

        public List<SpecialisationDto> GetSpecialisations(Expression<Func<Specialisation, bool>> predicate)
        {
            return GetList(predicate, s => s.ProductType)
                .Select(s => new SpecialisationDto(s, true))
                .ToList();
        }

        public List<ShopDto> GetShops(int modelId, Expression<Func<ShopWallet, bool>> predicate)
        {
            return GetList(predicate, s => s.Owner.Sins, s => s.Wallet, s => s.Specialisations, s => s.TrustedUsers).Select(s =>
                        new ShopDto(modelId, s)).ToList();
        }

        public ShopDetailedDto GetDetailedShop(int shopId)
        {
            var shop = Get<ShopWallet>(s => s.Id == shopId, s => s.Owner.Sins, s => s.Wallet, s => s.Specialisations, s => s.TrustedUsers);
            if (shop == null)
                throw new BillingNotFoundException($"Магазин {shopId} не найден");
            var sIds = shop.Specialisations.Select(s => s.SpecialisationId);
            var specialisations = GetList<Specialisation>(s => sIds.Contains(s.Id));
            var products = GetAvailableQR(shopId);
            return new ShopDetailedDto(shop, products, specialisations);
        }

        public List<CorporationDto> GetCorporationDtos(Expression<Func<CorporationWallet, bool>> predicate)
        {
            return GetList(predicate, c => c.Wallet, c => c.Owner.Sins, s => s.Specialisations).Select(c =>
                      new CorporationDto(c)).ToList();
        }

        public CorporationDetailedDto GetCorporationDto(int corporationId)
        {
            var corporation = Get<CorporationWallet>(s => s.Id == corporationId, s => s.Specialisations);
            if (corporation == null)
                throw new BillingNotFoundException($"Корпорация {corporationId} не найдена");

            var specialisationIds = corporation.Specialisations.Select(s => s.SpecialisationId);
            var specialisations = GetList<Specialisation>(s => specialisationIds.Contains(s.Id));
            return new CorporationDetailedDto(corporation, specialisations);
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

        public void AddCorpSpecialisation(int corporation, int specialisation, decimal ratio)
        {
            var corp = Get<CorporationWallet>(c => c.Id == corporation);
            if (corp == null)
            {
                throw new BillingNotFoundException($"corporation {corporation} not found");
            }
            var dbspec = Get<Specialisation>(s => s.Id == specialisation);
            if(dbspec== null)
            {
                throw new BillingNotFoundException($"specialisation {specialisation} not found");
            }
            var corpspec = Get<CorporationSpecialisation>(c => c.SpecialisationId == specialisation && c.CorporationId == corporation);
            if(corpspec == null)
            {
                corpspec = new CorporationSpecialisation { CorporationId = corporation, SpecialisationId = specialisation };
            }
            corpspec.Ratio = ratio;
            SaveContext();
        }

        public void DeleteCorpSpecialisation(int corporation, int specialisation)
        {
            var corpspec = Get<CorporationSpecialisation>(c => c.SpecialisationId == specialisation && c.CorporationId == corporation);
            if (corpspec == null)
            {
                throw new BillingNotFoundException($"corporationspecialisation {corporation}-{specialisation} not found");
            }
            RemoveAndSave(corpspec);
        }

        public List<ProductTypeDto> GetProductTypes(Expression<Func<ProductType, bool>> predicate)
        {
            return GetList(predicate).Select(p =>
                new ProductTypeDto(p, true)).ToList();
        }

        public List<NomenklaturaDto> GetNomenklaturas(Expression<Func<Nomenklatura, bool>> predicate)
        {
            var list = GetListAsNoTracking(predicate, n => n.Specialisation.ProductType);
            return list.Select(s => new NomenklaturaDto(s, true)).ToList();
        }

        public List<SkuDto> GetSkus(Expression<Func<Sku, bool>> predicate)
        {
            var list = GetListAsNoTracking(predicate, s => s.Corporation, s => s.Nomenklatura, s => s.Nomenklatura.Specialisation.ProductType);
            return list.Select(s => new SkuDto(s, true)).ToList();
        }

        public List<UserDto> GetUsers()
        {
            var list = GetActiveSins(u => u.Character, u => u.Wallet, u => u.Passport); 
            return list.Select(c => new UserDto(c)).ToList();
        }

        public List<SIN> GetActiveSins(params Expression<Func<SIN, object>>[] includes)
        {
            var currentGame = 1;
            var sins = GetListAsNoTracking(s => (s.InGame ?? false) && s.Character.Game == currentGame && (s.EVersion == "4" || s.EVersion == "3"), includes);
            return sins;
        }

        public ShopDto CreateOrUpdateShopWallet(int shopId = 0, decimal balance = 0, string name = "default shop", int lifestyle = 1, int ownerId = 0, List<int> specialisations = null, string comment = "", string location = "")
        {
            ShopWallet shop = null;
            if (shopId == 0)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Shop);
                shop = new ShopWallet
                {
                    Wallet = newWallet,
                    OwnerId = ownerId
                };
                AddAndSave(shop);
                shopId = shop.Id;
            }
            else
            {
                shop = Get<ShopWallet>(w => w.Id == shopId, s => s.Wallet, s => s.Specialisations);
            }
            if (shop == null)
            {
                throw new BillingException("shop not found");
            }
            shop.Name = name;
            shop.OwnerId = ownerId;
            shop.Wallet.Balance = balance;
            shop.Comment = comment;
            shop.Location = location;
            var ls = BillingHelper.GetLifestyle(lifestyle);
            shop.LifeStyle = (int)ls;
            SaveContext();
            var dbSpecialisations = GetList<ShopSpecialisation>(s => s.ShopId == shop.Id);
            foreach (var shopspecialisation in dbSpecialisations)
            {
                Remove(shopspecialisation);
            }
            SaveContext();
            if (specialisations != null)
            {
                foreach (var specialisationId in specialisations)
                {
                    var specialisation = Get<Specialisation>(s => s.Id == specialisationId);
                    if (specialisation == null)
                        throw new Exception($"некорректные входные данные specialisation: {specialisation?.Id} ");
                    Add(new ShopSpecialisation { ShopId = shopId, SpecialisationId = specialisationId });
                }
                SaveContext();
            }
            shop = GetAsNoTracking<ShopWallet>(w => w.Id == shopId, s => s.Owner, s => s.Wallet, s => s.Specialisations);
            var dto = new ShopDto(ownerId, shop);
            return dto;
        }

        public SpecialisationDto CreateOrUpdateSpecialisation(int id, int producttype, string name)
        {
            Specialisation specialisation = null;
            if (id > 0)
                specialisation = Get<Specialisation>(s => s.Id == id);
            else
            {
                specialisation = new Specialisation();
            }
            specialisation.Name = name;
            var product = Get<ProductType>(p => p.Id == producttype);
            if (product == null)
                throw new Exception("producttype not found");
            specialisation.ProductTypeId = producttype;
            Add(specialisation);
            SaveContext();
            var dbspec = Get<Specialisation>(s => s.Id == specialisation.Id, s => s.ProductType);
            if (dbspec == null)
                throw new Exception("Ошибка добавления специализации");
            return new SpecialisationDto(dbspec, true);
        }

        public NomenklaturaDto CreateOrUpdateNomenklatura(int id, string name, string code, int specialisationId, int lifestyle, decimal baseprice, int basecount, string description, string pictureurl, int externalId = 0)
        {
            Nomenklatura nomenklatura = null;
            if (id > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == id);
            if (nomenklatura == null)
            {
                nomenklatura = new Nomenklatura();
                nomenklatura.PictureUrl = UrlNotFound;
                nomenklatura.Code = string.Empty;
                Add(nomenklatura);
            }
            nomenklatura.Name = name;
            nomenklatura.Code = code;
            nomenklatura.BasePrice = baseprice;
            nomenklatura.Description = description;
            nomenklatura.PictureUrl = pictureurl;
            if (externalId != 0)
            {
                nomenklatura.ExternalId = externalId;
            }
            nomenklatura.BaseCount = basecount;
            Specialisation specialisation = null;
            if (specialisationId > 0)
                specialisation = Get<Specialisation>(p => p.Id == specialisationId);
            else
                specialisation = Get<Specialisation>(p => p.Id == nomenklatura.SpecialisationId);
            if (specialisation == null)
            {
                throw new BillingException("specialisation not found");
            }
            nomenklatura.SpecialisationId = specialisation.Id;
            nomenklatura.Lifestyle = lifestyle;
            nomenklatura.Lifestyle = (int)BillingHelper.GetLifestyle(nomenklatura.Lifestyle);
            SaveContext();
            nomenklatura = GetAsNoTracking<Nomenklatura>(n => n.Id == nomenklatura.Id, n => n.Specialisation.ProductType);
            if (nomenklatura == null)
                throw new Exception("Создать nomenklatura не получилось");
            return new NomenklaturaDto(nomenklatura, true);
        }

        public SkuDto CreateOrUpdateSku(int id, int nomenklaturaid, int count, int corporationid, string name, bool enabled, int? skubaseprice = null, int? skubasecount = null)
        {
            Sku sku = null;
            if (id > 0)
                sku = Get<Sku>(s => s.Id == id);
            if (sku == null)
            {
                sku = new Sku();
                Add(sku);
            }
            sku.Enabled = enabled;
            sku.SkuBasePrice = skubaseprice;
            sku.SkuBaseCount = skubasecount;
            if (count > 0)
                sku.Count = count;
            if (!string.IsNullOrEmpty(name))
                sku.Name = name;
            CorporationWallet corporation = null;
            if (corporationid > 0)
                corporation = Get<CorporationWallet>(c => c.Id == corporationid);
            else
                corporation = Get<CorporationWallet>(c => c.Id == sku.CorporationId);
            if (corporation == null)
            {
                throw new BillingException("Corporation not found");
            }

            sku.CorporationId = corporation.Id;
            Nomenklatura nomenklatura = null;
            if (nomenklaturaid > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == nomenklaturaid);
            else
                nomenklatura = Get<Nomenklatura>(n => n.Id == sku.NomenklaturaId);
            if (nomenklatura == null)
            {
                throw new BillingException("Nomenklatura not found");
            }
            sku.NomenklaturaId = nomenklatura.Id;
            SaveContext();
            sku = GetAsNoTracking<Sku>(s => s.Id == sku.Id, s => s.Nomenklatura.Specialisation.ProductType);
            if (sku == null)
                throw new Exception("создать sku не получилось");
            return new SkuDto(sku, true);
        }

        public void DeleteSpecialisation(int id)
        {
            var nomenklaturas = GetList<Nomenklatura>(n => n.SpecialisationId == id);
            if (nomenklaturas != null && nomenklaturas.Count > 0)
            {
                throw new Exception($"Сперва необходимо удалить номенклатуры с id {string.Join(";", nomenklaturas)}");
            }
            DeleteAndSave<Specialisation>(id);
        }

        public void DeleteNomenklatura(int id)
        {
            var skus = GetList<Sku>(s => s.NomenklaturaId == id);
            if (skus != null && skus.Count > 0)
            {
                throw new Exception($"Сперва необходимо удалить ску с id {string.Join(";", skus)}");
            }
            DeleteAndSave<Nomenklatura>(id);
        }

        public void DeleteShop(int shopid)
        {
            var shop = Get<ShopWallet>(c => c.Id == shopid);
            if (shop == null)
                throw new Exception("shop not found");
            var contracts = GetList<Contract>(c => c.ShopId == shopid);
            if (contracts != null && contracts.Count > 0)
            {
                throw new Exception("Сперва необходимо убрать все контракты магазина");
            }
            var specialisations = GetList<ShopSpecialisation>(s => s.ShopId == shopid);
            foreach (var specialisation in specialisations)
            {
                Remove(specialisation);
            }
            var wallet = Get<Wallet>(w => w.Id == shop.WalletId);
            Remove(shop);
            Remove(wallet);
            SaveContext();
        }

        public void DeleteSku(int skuId)
        {
            var sku = Get<Sku>(s => s.Id == skuId);
            if (sku == null)
                throw new Exception("sku not found");
            DeleteAndSave<Sku>(skuId);
        }

        public CorporationWallet CreateOrUpdateCorporationWallet(int corpId = 0, decimal amount = 0, string name = "unknown corporation", string logoUrl = "")
        {
            CorporationWallet corporation = null;
            if (corpId > 0)
                corporation = Get<CorporationWallet>(w => w.Id == corpId, c => c.Wallet);
            if (corporation == null)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Corporation);
                corporation = new CorporationWallet
                {
                    Wallet = newWallet,
                    Id = corpId,
                    CorporationLogoUrl = UrlNotFound
                };
                Add(corporation);
            }
            if (!string.IsNullOrEmpty(logoUrl))
                corporation.CorporationLogoUrl = logoUrl;
            if (!string.IsNullOrEmpty(name))
                corporation.Name = name;
            if (amount > 0)
                corporation.Wallet.Balance = amount;
            SaveContext();
            return corporation;
        }

        public Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment)
        {
            var d1 = BillingBlocked(characterFrom, s => s.Wallet, s => s.Character);
            var d2 = BillingBlocked(characterTo, s => s.Wallet, s => s.Character);
            return MakeTransferSINSIN(d1, d2, amount, comment);
        }

        public Transfer MakeTransferSINSIN(int characterFrom, string sinTo, decimal amount, string comment)
        {
            var d1 = BillingBlocked(characterFrom, s => s.Wallet, s => s.Character);
            var d2 = BillingBlocked(sinTo, s => s.Wallet, s => s.Character);
            return MakeTransferSINSIN(d1, d2, amount, comment);
        }

        public void CleanRenta(Renta renta)
        {
            if (!string.IsNullOrEmpty(renta.QRRecorded))
            {
                var ereminService = new EreminService();
                ereminService.CleanQR(renta.QRRecorded).GetAwaiter().GetResult();
                renta.QRRecorded = $"deleted {renta.QRRecorded}";
            }
            renta.Expired = true;
            SaveContext();
        }

        public void UpdateShopTrustedUsers(int shopId, List<int> trustedModels)
        {
            var shop = Get<ShopWallet>(s => s.Id == shopId);
            if (shop == null)
            {
                throw new BillingNotFoundException($"shop {shopId} not found");
            }
            foreach (var model in trustedModels)
            {
                var sin = Get<SIN>(s => s.Character.Model == model);
                if (sin == null)
                {
                    throw new BillingNotFoundException($"model {model} not found");
                }
            }
            var oldDB = GetList<ShopTrusted>(t => t.ShopId == shopId);
            RemoveRange(oldDB);
            SaveContext();
            foreach (var model in trustedModels)
            {
                var newdb = new ShopTrusted { Model = model, ShopId = shopId };
                Add(newdb);
            }
            SaveContext();
        }

        protected List<TransferDto> CreateTransfersDto(List<Transfer> listFrom, List<Transfer> listTo, string owner)
        {
            var walletdtos = listFrom.GroupBy(t => new { t.WalletToId, t.WalletTo.WalletType }).Select(t => new WalletDto { WalletId = t.Key.WalletToId, WalletType = (WalletTypes)t.Key.WalletType }).ToList();
            walletdtos.AddRange(listTo.GroupBy(t => new { t.WalletFromId, t.WalletFrom.WalletType }).Select(t => new WalletDto { WalletId = t.Key.WalletFromId, WalletType = (WalletTypes)t.Key.WalletType }));
            var sinIds = walletdtos.Where(w => w.WalletType == WalletTypes.Character).Select(w => w.WalletId).ToList();
            var sins = GetListAsNoTracking<SIN>(s => sinIds.Contains(s.WalletId ?? 0), s => s.Passport);
            var shopIds = walletdtos.Where(w => w.WalletType == WalletTypes.Shop).Select(w => w.WalletId).ToList();
            var shops = GetListAsNoTracking<ShopWallet>(s => shopIds.Contains(s.WalletId ?? 0));
            var allList = new List<TransferDto>();
            allList.AddRange(listFrom
                .Select(s => CreateTransferDto(s, TransferType.Outcoming, sins, shops, owner))
                .ToList());
            allList.AddRange(listTo
                .Select(s => CreateTransferDto(s, TransferType.Incoming, sins, shops, owner))
                .ToList());
            return allList.OrderByDescending(t => t.OperationTime).ToList();
        }

        protected List<TransferDto> CreateTransfersDto(List<Transfer> list, string owner, TransferType type)
        {
            var walletdtos = list.GroupBy(t => new { t.WalletToId, t.WalletTo.WalletType }).Select(t => new WalletDto { WalletId = t.Key.WalletToId, WalletType = (WalletTypes)t.Key.WalletType }).ToList();
            var sinIds = walletdtos.Where(w => w.WalletType == WalletTypes.Character).Select(w => w.WalletId).ToList();
            var sins = GetListAsNoTracking<SIN>(s => sinIds.Contains(s.WalletId ?? 0), s => s.Passport);
            var shopIds = walletdtos.Where(w => w.WalletType == WalletTypes.Shop).Select(w => w.WalletId).ToList();
            var shops = GetListAsNoTracking<ShopWallet>(s => shopIds.Contains(s.WalletId ?? 0));
            var allList = new List<TransferDto>();
            allList.AddRange(list
                .Select(s => CreateTransferDto(s, type, sins, shops, owner))
                .ToList());
            return allList.OrderByDescending(t => t.OperationTime).ToList();
        }


        protected Transfer MakeTransferSINSIN(SIN sinFrom, SIN sinTo, decimal amount, string comment, bool allAnon = false)
        {
            var anonFrom = GetAnon(sinFrom.Character.Model);
            var anonto = GetAnon(sinTo.Character.Model);
            var anon = allAnon|| anonFrom || anonto;
            var transfer = AddNewTransfer(sinFrom.Wallet, sinTo.Wallet, amount, comment, anon);
            SaveContext();
            if (transfer != null)
            {
                EreminPushAdapter.SendNotification(sinTo.Character.Model, "Кошелек", $"Вам переведено денег {amount}");
            }
            return transfer;
        }

        protected Transfer MakeTransferLegSIN(int shopId, SIN sin, decimal amount, string comment)
        {
            var anon = GetAnon(sin.Character.Model);
            var shop = Get<ShopWallet>(s => s.Id == shopId, s => s.Wallet);
            if (shop == null)
                throw new BillingNotFoundException($"shop {shopId} not found");
            var transfer = AddNewTransfer(shop.Wallet, sin.Wallet, amount, comment, anon);
            SaveContext();
            return transfer;
        }

        private List<SkuDto> GetSkusForShop(int shop)
        {
            return GetSkuList(shop, s => s.Corporation.Wallet, s => s.Nomenklatura.Specialisation.ProductType).Select(s => new SkuDto(s, true)).ToList();
        }

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
                SaveContext();
            }
            qr.QR = cache.Encoded;
            return qr;
        }
    }
}
