using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using InternalServices;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public interface IBillingManager : IBaseRepository
    {
        #region application
        Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment);
        Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);

        string GetSinStringByCharacter(int characterId);
        int GetCharacterIdBySin(string sinString);
        List<TransferDto> GetTransfers(int characterId);
        BalanceDto GetBalance(int characterId);
        List<RentaDto> GetRentas(int characterId);
        #endregion

        #region web
        List<PriceShopDto> GetOffersForQR(int characterId);
        PriceShopDto GetPriceByQR(int character, int qr);
        PriceShopDto GetPrice(int character, int shop, int sku);
        Specialisation SetSpecialisation(int productType, int shop);
        void DropSpecialisation(int productType, int shop);
        void BreakContract(int corporation, int shop);
        Contract CreateContract(int corporation, int shop);
        RentaDto ConfirmRenta(int character, int priceId);
        List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled, int id = -1);
        List<CorporationDto> GetCorps();
        List<ProductTypeDto> GetProductTypes(int id = -1);
        ProductType GetExtProductType(int externalId);
        Nomenklatura GetExtNomenklatura(int externalId);
        Sku GetExtSku(int externalId);
        List<NomenklaturaDto> GetNomenklaturas(int producttype, int lifestyle, int id = -1);
        List<Contract> GetContrats(int shopid, int corporationId);
        #endregion

        #region jobs
        void ProcessRentas();
        #endregion

        #region admin
        Sku CreateOrUpdateSku(int id, int nomenklatura, int count, int corporation, string name, bool enabled, int externalId = 0);
        Nomenklatura CreateOrUpdateNomenklatura(int id, string name, string code, int producttype, int lifestyle, decimal baseprice, string description, string pictureurl, int externalId = 0);
        SIN CreateOrUpdatePhysicalWallet(int character, decimal balance);
        ProductType CreateOrUpdateProductType(int id, string name, int discounttype = 1, int externalId = 0);
        CorporationWallet CreateOrUpdateCorporationWallet(int id, decimal amount, string name, string logoUrl);
        ShopWallet CreateOrUpdateShopWallet(int foreignKey, decimal amount, string name, int lifestyle);
        void DeleteCorporation(int corpid);
        void DeleteShop(int shopid);
        void DeleteProductType(int id, bool force);
        void DeleteNomenklatura(int id, bool force);
        #endregion
    }

    public class BillingManager : BaseBillingRepository, IBillingManager
    {
        public static string UrlNotFound = "https://www.clickon.ru/preview/original/pic/8781_logo.png";

        public void ProcessRentas()
        {
            var rentas = GetList<Renta>(r => true, r => r.Shop.Wallet, r => r.Sku.Nomenklatura.ProductType, r => r.Sku.Corporation.Wallet);
            var bulkCount = 500;
            var pageCount = (rentas.Count + bulkCount - 1) / bulkCount;
            for (int i = 0; i < pageCount; i++)
            {
                ProcessBulk(rentas.Skip(i * bulkCount).Take(bulkCount).ToList());
            }
        }

        public Specialisation SetSpecialisation(int productTypeid, int shopid)
        {
            var specialisation = Get<Specialisation>(s => s.ProductTypeId == productTypeid && s.ShopId == shopid);
            if (specialisation != null)
                throw new BillingException("У магазина уже есть эта специализация");
            var producttype = Get<ProductType>(p => p.Id == productTypeid);
            if (producttype == null)
                throw new BillingException("ProductType не найден");
            var shop = Get<ShopWallet>(s => s.Id == shopid);
            if (shop == null)
                throw new BillingException("shop не найден");
            specialisation = new Specialisation
            {
                ProductTypeId = producttype.Id,
                ShopId = shop.Id
            };
            Add(specialisation);
            Context.SaveChanges();
            return specialisation;
        }
        
        public void DropSpecialisation(int productType, int shop)
        {
            var specialisation = Get<Specialisation>(s => s.ProductTypeId == productType && s.ShopId == shop);
            if (specialisation == null)
                throw new BillingException("У магазина нет указанной специализации");
            Remove(specialisation);
            Context.SaveChanges();
        }

        public void BreakContract(int corporation, int shop)
        {
            var contract = Get<Contract>(c => c.CorporationId == corporation && c.ShopId == shop);
            if (contract == null)
                throw new BillingException("Контракт не найден");
            Remove(contract);
            Context.SaveChanges();
        }

        public Contract CreateContract(int corporation, int shop)
        {
            var contract = Get<Contract>(c => c.CorporationId == corporation && c.ShopId == shop);
            if (contract != null)
                throw new BillingException("Контракт уже заключен");
            //TODO CONTRACT LIMIT
            var newContract = new Contract
            {
                ShopId = shop,
                CorporationId = corporation
            };
            Add(newContract);
            Context.SaveChanges();
            return newContract;
        }

        public List<NomenklaturaDto> GetNomenklaturas(int producttype, int lifestyle, int id = -1)
        {
            var list = GetList<Nomenklatura>(n =>
                (n.ProductTypeId == producttype || producttype == 0)
                && (n.Lifestyle == lifestyle || lifestyle == 0)
                && (n.Id == id || id == -1)
                , n => n.ProductType);
            return list.Select(s => new NomenklaturaDto(s)).ToList();
        }

        public List<Contract> GetContrats(int shopid, int corporationId)
        {
            var list = GetList<Contract>(c => (c.ShopId == shopid || shopid == 0) && (c.CorporationId == corporationId || corporationId == 0));
            return list;
        }

        public List<ProductTypeDto> GetProductTypes(int id = -1)
        {
            return GetList<ProductType>(p => p.Id == id || id == -1).Select(p =>
                new ProductTypeDto(p)).ToList();
        }

        public ProductType GetExtProductType(int externalId)
        {
            return Get<ProductType>(p => p.ExternalId == externalId);
        }

        public Nomenklatura GetExtNomenklatura(int externalId)
        {
            return Get<Nomenklatura>(p => p.ExternalId == externalId);
        }

        public Sku GetExtSku(int externalId)
        {
            return Get<Sku>(p => p.ExternalId == externalId);
        }

        public List<CorporationDto> GetCorps()
        {
            return GetList<CorporationWallet>(c => true, c => c.Wallet).Select(c =>
                  new CorporationDto
                  {
                      Id = c.Id,
                      Name = c.Name,
                      Balance = c.Wallet.Balance,
                      CorporationUrl = c.CorporationLogoUrl
                  }).ToList();
        }
        
        public List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled, int id = -1)
        {
            var list = GetList<Sku>(s => (s.CorporationId == corporationId || corporationId == 0)
                && (s.NomenklaturaId == nomenklaturaId || nomenklaturaId == 0)
                && (s.Enabled == enabled || !enabled.HasValue)
                && (s.Id == id || id == -1)
                , s => s.Corporation, s => s.Nomenklatura, s => s.Nomenklatura.ProductType);
            return list.Select(s => new SkuDto(s)).ToList();
        }

        public List<RentaDto> GetRentas(int characterId)
        {
            var list = GetList<Renta>(r => r.CharacterId == characterId, r => r.Sku.Nomenklatura.ProductType, r => r.Sku.Corporation, r => r.Shop);
            return list
                    .Select(r =>
                    new RentaDto
                    {
                        FinalPrice = BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring),
                        ProductType = r.Sku.Nomenklatura.ProductType.Name,
                        Shop = r.Shop.Name,
                        NomenklaturaName = r.Sku.Nomenklatura.Name,
                        SkuName = r.Sku.Name,
                        Corporation = r.Sku.Corporation.Name
                    }).ToList();
        }

        public RentaDto ConfirmRenta(int character, int priceId)
        {
            var block = _settings.GetBoolValue(SystemSettingsEnum.block);
            if (block)
                throw new ShopException("В данный момент ведется пересчет рентных платежей, попробуйте купить чуть позже");
            var price = Get<Price>(p => p.Id == priceId, p => p.Sku, s => s.Shop, s => s.Shop.Wallet);
            if (price == null)
                throw new BillingException("Персональное предложение не найдено");
            if (price.Confirmed)
                throw new Exception("Персональным предложением уже воспользовались");
            if (price.CharacterId != character)
                throw new Exception("Персональное предложение заведено на другого персонажа");
            var dateTill = price.DateCreated.AddMinutes(_settings.GetIntValue(SystemSettingsEnum.price_minutes));
            if (dateTill < DateTime.Now)
                throw new BillingException($"Персональное предложение больше не действительно, оно истекло {dateTill.ToString("HH:mm:ss")}");
            var sku = SkuAllowed(price.ShopId, price.SkuId);
            if (sku == null)
                throw new BillingException("Sku недоступно для продажи в данный момент");
            var sin = GetSIN(price.CharacterId, s => s.Wallet);
            if (sin.Wallet.Balance - price.FinalPrice < 0)
            {
                throw new BillingException("Недостаточно средств");
            }
            var mir = GetMIR();
            MakeNewTransfer(sin.Wallet, mir, price.FinalPrice, $"Первый платеж за: {price.Sku.Name}, место покупки: {price.Shop.Name}");
            MakeNewTransfer(mir, price.Shop.Wallet, price.ShopComission, $"Комиссия за: {price.Sku.Name} с син {sin.CharacterId}");
            MakeNewTransfer(mir, sku.Corporation.Wallet, price.BasePrice, $"Покупка предмета: {price.Sku.Name} пользователем: {sin.CharacterId}");
            sku.Count--;
            Add(sku);
            var renta = new Renta
            {
                BasePrice = price.BasePrice,
                CharacterId = sin.CharacterId,
                CurrentScoring = price.CurrentScoring,
                SkuId = price.SkuId,
                DateCreated = DateTime.Now,
                Discount = price.Discount,
                ShopComission = price.ShopComission,
                ShopId = price.ShopId,
                HasQRWrite = BillingHelper.HasQrWrite(sku.Nomenklatura.Code),
                PriceId = priceId
            };
            Add(renta);
            price.Confirmed = true;
            Add(price);
            Context.SaveChanges();
            var dto = new RentaDto
            {
                HasQRWrite = renta.HasQRWrite,
                OfferId = priceId
            };
            return dto;
        }
        
        public List<PriceShopDto> GetOffersForQR(int characterId)
        {
            var rentas = GetList<Renta>(r => r.CharacterId == characterId && r.HasQRWrite, r => r.Price.Sku.Nomenklatura.ProductType, r => r.Price.Sku.Corporation, r => r.Price.Shop);
            var list = rentas.Select(r =>
                        new PriceShopDto(new PriceDto(r.Price))
            );
            return list.ToList();
        }

        public PriceShopDto GetPriceByQR(int character, int qrid)
        {
            var qr = Get<ShopQR>(q => q.Id == qrid);
            if (qr == null)
                throw new BillingException($"qr {qrid} not found");
            if (qr.ShopId == null || qr.SkuId == null)
                throw new BillingException("пустой qr");
            return GetPrice(character, qr.ShopId.Value, qr.SkuId.Value);
        }
        
        public PriceShopDto GetPrice(int character, int shopid, int skuid)
        {
            var block = _settings.GetBoolValue(SystemSettingsEnum.block);
            if (block)
                throw new ShopException("В данный момент ведется пересчет рентных платежей, попробуйте получить цену чуть позже");
            var sku = SkuAllowed(shopid, skuid);
            if (sku == null)
                throw new BillingException("sku недоступен для продажи");
            var shop = Get<ShopWallet>(s => s.Id == shopid);
            var sin = GetSIN(character, s => s.Scoring);
            if (shop == null || sin == null)
                throw new Exception("some went wrong");
            var price = CreateNewPrice(sku, shop, sin);
            var dto = new PriceShopDto(new PriceDto(price));
            return dto;
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
            }
            if (!string.IsNullOrEmpty(logoUrl))
                corporation.CorporationLogoUrl = logoUrl;
            if (!string.IsNullOrEmpty(name))
                corporation.Name = name;
            if (amount > 0)
                corporation.Wallet.Balance = amount;
            Add(corporation);
            Context.SaveChanges();
            return corporation;
        }

        public ShopWallet CreateOrUpdateShopWallet(int shopId = 0, decimal amount = 0, string name = "default shop", int lifestyle = 1)
        {
            ShopWallet shop = null;
            if (shopId > 0)
                shop = Get<ShopWallet>(w => w.Id == shopId, s => s.Wallet);
            if (shop == null)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Shop);
                shop = new ShopWallet
                {
                    Wallet = newWallet,
                    Id = shopId
                };
            }
            shop.Name = name;
            shop.Wallet.Balance = amount;
            shop.LifeStyle = (int)BillingHelper.GetLifestyle(lifestyle);
            Add(shop);
            Context.SaveChanges();
            return shop;
        }

        public void DeleteCorporation(int corpid)
        {
            var corporation = Get<CorporationWallet>(c => c.Id == corpid);
            if (corporation == null)
                throw new Exception("corporation not found");
            var wallet = Get<Wallet>(w => w.Id == corporation.WalletId);
            Remove(corporation);
            Remove(wallet);
            Context.SaveChanges();
        }

        public void DeleteShop(int shopid)
        {
            var shop = Get<ShopWallet>(c => c.Id == shopid);
            if (shop == null)
                throw new Exception("shop not found");
            var wallet = Get<Wallet>(w => w.Id == shop.WalletId);
            Remove(shop);
            Remove(wallet);
            Context.SaveChanges();
        }
        
        public void DeleteProductType(int id, bool force)
        {
            var nomenklaturas = GetList<Nomenklatura>(n => n.ProductTypeId == id);
            if(nomenklaturas != null && nomenklaturas.Count > 0)
            {
                if (force)
                {
                    foreach (var nomenklatura in nomenklaturas)
                    {
                        DeleteNomenklatura(nomenklatura.Id, true);
                    }
                }
                else
                {
                    throw new Exception($"Сперва необходимо удалить номенклатуры ссылающиеся на этот тип товара");
                }
            }
            Delete<ProductType>(id);
        }

        public void DeleteNomenklatura(int id, bool force)
        {
            var skus = GetList<Sku>(s => s.NomenklaturaId == id);
            if(skus != null && skus.Count > 0)
            {
                if(force)
                {
                    foreach (var sku in skus)
                    {
                        Delete<Sku>(sku.Id);
                    }
                }
                else
                {
                    throw new Exception("Сперва необходимо удалить ску ссылающиеся на эту номенклатуру");
                }
            }
            Delete<Nomenklatura>(id);
        }

        public BalanceDto GetBalance(int characterId)
        {
            var sin = GetSIN(characterId, s => s.Wallet, s => s.Scoring);
            var balance = new BalanceDto
            {
                CharacterId = characterId,
                CurrentBalance = sin.Wallet.Balance,
                CurrentScoring = sin.Scoring.CurrentScoring,
                SIN = sin.Sin,
                ForecastLifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.Balance).ToString(),
                LifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.Balance).ToString()
            };
            return balance;
        }

        public List<TransferDto> GetTransfers(int characterId)
        {
            var sin = GetSIN(characterId);
            if (sin == null)
                throw new BillingException("sin not found");
            var listFrom = GetList<Transfer>(t => t.WalletFromId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            var allList = new List<TransferDto>();
            var owner = GetWalletName(sin.Wallet);
            if (listFrom != null)
                allList.AddRange(listFrom
                    .Select(s => CreateTransferDto(s, TransferType.Outcoming, owner))
                    .ToList());
            var listTo = GetList<Transfer>(t => t.WalletToId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            if (listTo != null)
                allList.AddRange(listTo
                    .Select(s => CreateTransferDto(s, TransferType.Incoming, owner))
                    .ToList());
            return allList.OrderBy(t => t.OperationTime).ToList();
        }

        public string GetSinStringByCharacter(int characterId)
        {
            var sin = GetSIN(characterId);
            if (sin == null)
                throw new Exception("sin not found");
            return sin.Sin;
        }

        public int GetCharacterIdBySin(string sinString)
        {
            var sin = Get<SIN>(s => s.Sin == sinString);
            if (sin == null)
                throw new Exception("sin not found");
            return sin.CharacterId;
        }

        public ProductType CreateOrUpdateProductType(int id, string name, int discounttype = 1, int externalId = 0)
        {
            ProductType type = null;
            if (id > 0)
                type = Get<ProductType>(p => p.Id == id);
            if (type == null)
            {
                type = new ProductType();
            }
            if (discounttype != 0)
                type.DiscountType = discounttype;
            if (!Enum.IsDefined(typeof(DiscountType), type.DiscountType))
            {
                type.DiscountType = (int)DiscountType.Gesheftmaher;
            }
            if (externalId != 0)
            {
                type.ExternalId = externalId;
            }
            if (!string.IsNullOrEmpty(name))
                type.Name = name;
            Add(type);
            Context.SaveChanges();
            return type;
        }

        public Nomenklatura CreateOrUpdateNomenklatura(int id, string name, string code, int producttypeid, int lifestyle, decimal baseprice, string description, string pictureurl, int externalId = 0)
        {
            Nomenklatura nomenklatura = null;
            if (id > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == id);
            if (nomenklatura == null)
            {
                nomenklatura = new Nomenklatura();
                nomenklatura.PictureUrl = UrlNotFound;
                nomenklatura.Code = string.Empty;
            }
            if (!string.IsNullOrEmpty(name))
                nomenklatura.Name = name;
            if (!string.IsNullOrEmpty(code))
                nomenklatura.Code = code;
            if (baseprice > 0)
                nomenklatura.BasePrice = baseprice;
            if (!string.IsNullOrEmpty(description))
                nomenklatura.Description = description;
            if (!string.IsNullOrEmpty(pictureurl))
                nomenklatura.PictureUrl = pictureurl;
            if (externalId != 0)
            {
                nomenklatura.ExternalId = externalId;
            }
            ProductType producttype = null;
            if (producttypeid > 0)
                producttype = Get<ProductType>(p => p.Id == producttypeid);
            else
                producttype = Get<ProductType>(p => p.Id == nomenklatura.ProductTypeId);
            if (producttype == null)
            {
                throw new BillingException("ProductType not found");
            }
            nomenklatura.ProductTypeId = producttype.Id;
            if (lifestyle > 0)
                nomenklatura.Lifestyle = lifestyle;
            nomenklatura.Lifestyle = (int)BillingHelper.GetLifestyle(nomenklatura.Lifestyle);
            Add(nomenklatura);
            Context.SaveChanges();
            return nomenklatura;
        }

        public Sku CreateOrUpdateSku(int id, int nomenklaturaid, int count, int corporationid, string name, bool enabled, int externalId = 0)
        {
            Sku sku = null;
            if (id > 0)
                sku = Get<Sku>(s => s.Id == id);
            if (sku == null)
            {
                sku = new Sku();
            }
            sku.Enabled = enabled;
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
                //corporation = CreateOrUpdateCorporationWallet(corporationid);
            }

            sku.CorporationId = corporation.Id;
            if (externalId != 0)
            {
                sku.ExternalId = externalId;
            }
            Nomenklatura nomenklatura = null;
            if (nomenklaturaid > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == nomenklaturaid);
            else
                nomenklatura = Get<Nomenklatura>(n => n.Id == sku.NomenklaturaId);
            if (nomenklatura == null)
            {
                throw new BillingException("Nomenklatura not found");
                //nomenklatura = CreateOrUpdateNomenklatura(nomenklaturaid, "unknown nomenklatura", string.Empty, 0, 1, 0, "unknown nomenklatura", string.Empty);
            }
            sku.NomenklaturaId = nomenklatura.Id;
            Add(sku);
            Context.SaveChanges();
            return sku;
        }

        [BillingBlock]
        public Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

        [BillingBlock]
        public Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment)
        {
            var d1 = GetSIN(characterFrom, s => s.Wallet);
            var d2 = GetSIN(characterTo, s => s.Wallet);
            var anon = false;
            try
            {
                var anonFrom = EreminService.GetAnonimous(characterFrom);
                var anonto = EreminService.GetAnonimous(characterTo);
                anon = anonFrom || anonto;
            }
            catch (Exception e)
            {

            }
            var transfer = MakeNewTransfer(d1.Wallet, d2.Wallet, amount, comment, anon);
            if (transfer != null)
            {
                EreminPushAdapter.SendNotification(characterTo, "Кошелек", $"Вам переведено денег {amount}");
            }
            return transfer;
        }

        #region private

        private void ProcessBulk(List<Renta> rentas)
        {
            var mir = GetMIR();
            foreach (var renta in rentas)
            {
                try
                {
                    ProcessRenta(renta, mir);
                }
                catch (Exception e)
                {

                    Console.Error.WriteLine(e.Message);
                }
            }
            Context.SaveChanges();
        }

        private void ProcessRenta(Renta renta, Wallet mir)
        {
            var sin = GetSIN(renta.CharacterId, s => s.Wallet);
            var finalPrice = BillingHelper.GetFinalPrice(renta.BasePrice, renta.Discount, renta.CurrentScoring);
            var comission = BillingHelper.CalculateComission(renta.BasePrice, renta.ShopComission);
            //с кошелька списываем всегда
            MakeNewTransfer(sin.Wallet, mir, finalPrice, $"Рентный платеж: {renta.Sku.Name} в {renta.Shop.Name}", false, false);
            EreminPushAdapter.SendNotification(sin.CharacterId, "Кошелек", $"Списание {finalPrice} по рентному договору");
            //если баланс положительный
            if (sin.Wallet.Balance > 0)
            {
                MakeNewTransfer(mir, renta.Sku.Corporation.Wallet, renta.BasePrice, $"Рентное начисление: {renta.Sku.Name} с {sin.Sin} ", false, false);
                MakeNewTransfer(mir, renta.Shop.Wallet, finalPrice, $"Рентное начисление: {renta.Sku.Name} в {renta.Shop.Name} с {sin.Sin}", false, false);
            }
        }
        
        private DiscountType GetDiscountTypeForSku(Sku sku)
        {
            if (sku == null)
                throw new Exception("sku not found");
            var nomenklatura = sku.Nomenklatura;
            if (nomenklatura == null)
                nomenklatura = Get<Nomenklatura>(n => n.Id == sku.NomenklaturaId);
            if (nomenklatura == null)
                throw new Exception("Nomenklatura not found");
            var producttype = nomenklatura.ProductType;
            if (producttype == null)
                producttype = Get<ProductType>(p => p.Id == nomenklatura.ProductTypeId);
            if (producttype == null)
                throw new Exception("ProductType not found");
            return BillingHelper.GetDiscountType(producttype.DiscountType);

        }
        
        private Price CreateNewPrice(Sku sku, ShopWallet shop, SIN sin)
        {
            decimal discount;
            try
            {
                discount = EreminService.GetDiscount(sin.CharacterId, GetDiscountTypeForSku(sku));
            }
            catch (Exception e)
            {
                discount = 0;
            }

            var price = new Price
            {
                Sku = sku,
                Shop = shop,
                BasePrice = sku.Nomenklatura.BasePrice,
                CurrentScoring = sin.Scoring.CurrentScoring,
                DateCreated = DateTime.Now,
                Discount = discount,
                CharacterId = sin.CharacterId,
                ShopComission = shop.Commission,
                FinalPrice = BillingHelper.GetFinalPrice(sku.Nomenklatura.BasePrice, discount, sin.Scoring.CurrentScoring)
            };
            Add(price);
            Context.SaveChanges();
            return price;
        }
        #endregion
    }
}
