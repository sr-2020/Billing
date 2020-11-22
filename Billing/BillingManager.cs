using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using InternalServices;
using IoC;
using Scoringspace;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Billing
{
    public interface IBillingManager : IBaseBillingRepository
    {
        #region application
        Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment);
        Transfer CreateTransferSINSIN(string modelId, string characterTo, decimal amount, string comment);
        Transfer CreateTransferMIRSIN(string characterTo, decimal amount);
        Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);

        string GetSinStringByCharacter(int modelId);
        int GetModelIdBySinString(string sinString);
        List<TransferDto> GetTransfers(int modelId);
        BalanceDto GetBalance(int modelId);
        List<RentaDto> GetRentas(int modelId);
        #endregion

        #region web
        PriceShopDto GetPriceByQR(int character, string qrid);
        PriceShopDto GetPrice(int modelId, int shop, int sku);
        Specialisation SetSpecialisation(int productType, int shop);
        void DropSpecialisation(int productType, int shop);
        void BreakContract(int corporation, int shop);
        Contract CreateContract(int corporation, int shop);
        RentaDto ConfirmRenta(int modelId, int priceId);
        List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled, int id = -1);
        List<ProductTypeDto> GetProductTypes(int id = -1);
        ProductType GetExtProductType(string name);
        Nomenklatura GetExtNomenklatura(string name);
        Sku GetExtSku(string name);
        List<NomenklaturaDto> GetNomenklaturas(int producttype, int lifestyle, int id = -1);
        List<Contract> GetContrats(int shopid, int corporationId);
        #endregion

        #region jobs
        void ProcessPeriod(string model = "0");
        int ProcessRentas(List<SIN> sins);
        int ProcessKarma(List<SIN> sins, decimal k);
        int ProcessIkar(List<SIN> sins, decimal k);

        List<SIN> GetActiveSins();

        #endregion

        #region admin

        List<CharacterDto> GetCharacters();
        List<TransferDto> GetTransfersByRenta(int rentaID);
        Sku CreateOrUpdateSku(int id, int nomenklatura, int count, int corporation, string name, bool enabled, int externalId = 0);
        Nomenklatura CreateOrUpdateNomenklatura(int id, string name, string code, int producttype, int lifestyle, decimal baseprice, string description, string pictureurl, int externalId = 0);
        ProductType CreateOrUpdateProductType(int id, string name, int discounttype = 1, int externalId = 0);
        CorporationWallet CreateOrUpdateCorporationWallet(int id, decimal amount, string name, string logoUrl);
        ShopWallet CreateOrUpdateShopWallet(int foreignKey, decimal amount, string name, int lifestyle, int owner);
        void DeleteCorporation(int corpid);
        void DeleteShop(int shopid);
        void DeleteProductType(int id, bool force);
        void DeleteNomenklatura(int id, bool force);
        #endregion
    }

    public class BillingManager : BaseBillingRepository, IBillingManager
    {
        public static string UrlNotFound = "";

        public int ProcessIkar(List<SIN> sins, decimal k)
        {
            var count = 0;
            var mir = GetMIR();
            foreach (var sin in sins)
            {
                if((sin.IKAR ?? 0) == 0)
                {
                    continue;
                }
                try
                {
                    MakeNewTransfer(mir, sin.Wallet, sin.IKAR.Value * k, "Начисления по ИКАР");
                    count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            Context.SaveChanges();
            return count;
        }

        public int ProcessKarma(List<SIN> sins, decimal k)
        {
            var count = 0;
            var mir = GetMIR();
            foreach (var sin in sins)
            {
                try
                {
                    var character = sin.Character;
                    if (character == null)
                    {
                        character = GetAsNoTracking<Character>(c => c.Id == sin.CharacterId);
                    }
                    var model = EreminService.GetCharacter(character.Model);
                    var karma = model.workModel.karma.spent;
                    if (karma == 0)
                    {
                        continue;
                    }
                    MakeNewTransfer(mir, sin.Wallet, karma * k, "Рабочие начисления за экономический период");
                    count++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
            return count;
        }

        public void ProcessPeriod(string model = "0")
        {
            var modelId = BillingHelper.GetModelId(model);
            var sins = GetList<SIN>(s => (s.Character.Model == modelId || modelId == 0) && (s.InGame ?? false), s => s.Character);
            ProcessRentas(sins);
        }

        public int ProcessRentas(List<SIN> sins)
        {
            var bulkCount = 100;
            var pageCount = (sins.Count + bulkCount - 1) / bulkCount;
            var rentasCount = 0;
            for (int i = 0; i < pageCount; i++)
            {
                var mir = GetMIR();
                foreach (var sin in sins.Skip(i * bulkCount).Take(bulkCount).ToList())
                {
                    rentasCount += ProcessRentas(sin, mir);
                }
                Context.SaveChanges();
            }
            return rentasCount;
        }

        public List<SIN> GetActiveSins()
        {
            var currentGame = 2;
            var sins = GetList<SIN>(s => s.InGame ?? false && s.Character.Game == currentGame, s => s.Character, s => s.Wallet);
            return sins;
        }

        public Specialisation SetSpecialisation(int productTypeid, int shopid)
        {
            var specialisation = GetAsNoTracking<Specialisation>(s => s.ProductTypeId == productTypeid && s.ShopId == shopid);
            if (specialisation != null)
                throw new BillingException("У магазина уже есть эта специализация");
            var producttype = GetAsNoTracking<ProductType>(p => p.Id == productTypeid);
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
            var contract = GetAsNoTracking<Contract>(c => c.CorporationId == corporation && c.ShopId == shop);
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
            var list = GetListAsNoTracking<Nomenklatura>(n =>
                (n.ProductTypeId == producttype || producttype == 0)
                && (n.Lifestyle == lifestyle || lifestyle == 0)
                && (n.Id == id || id == -1)
                , n => n.ProductType);
            return list.Select(s => new NomenklaturaDto(s)).ToList();
        }

        public List<Contract> GetContrats(int shopid, int corporationId)
        {
            var list = GetListAsNoTracking<Contract>(c => (c.ShopId == shopid || shopid == 0) && (c.CorporationId == corporationId || corporationId == 0));
            return list;
        }

        public List<ProductTypeDto> GetProductTypes(int id = -1)
        {
            return GetListAsNoTracking<ProductType>(p => p.Id == id || id == -1).Select(p =>
                new ProductTypeDto(p)).ToList();
        }

        public ProductType GetExtProductType(string name)
        {
            return GetAsNoTracking<ProductType>(p => p.Name == name);
        }

        public Nomenklatura GetExtNomenklatura(string name)
        {
            return GetAsNoTracking<Nomenklatura>(p => p.Name == name);
        }

        public Sku GetExtSku(string name)
        {
            return GetAsNoTracking<Sku>(p => p.Name == name);
        }

        public List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled, int id = -1)
        {
            var list = GetListAsNoTracking<Sku>(s => (s.CorporationId == corporationId || corporationId == 0)
                && (s.NomenklaturaId == nomenklaturaId || nomenklaturaId == 0)
                && (s.Enabled == enabled || !enabled.HasValue)
                && (s.Id == id || id == -1)
                , s => s.Corporation, s => s.Nomenklatura, s => s.Nomenklatura.ProductType);
            return list.Select(s => new SkuDto(s)).ToList();
        }

        public List<RentaDto> GetRentas(int modelId)
        {
            var list = GetListAsNoTracking<Renta>(r => r.Sin.Character.Model == modelId, r => r.Sku.Nomenklatura.ProductType, r => r.Sku.Corporation, r => r.Shop);
            return list
                    .Select(r =>
                    new RentaDto
                    {
                        FinalPrice = BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring),
                        ProductType = r.Sku.Nomenklatura.ProductType.Name,
                        Shop = r.Shop.Name,
                        NomenklaturaName = r.Sku.Nomenklatura.Name,
                        SkuName = r.Sku.Name,
                        Corporation = r.Sku.Corporation.Name,
                        RentaId = r.Id,
                        HasQRWrite = r.HasQRWrite,
                        QRRecorded = r.QRRecorded,
                        DateCreated = r.DateCreated
                    }).ToList();
        }

        [BillingBlock]
        public RentaDto ConfirmRenta(int modelId, int priceId)
        {
            var price = Get<Price>(p => p.Id == priceId, p => p.Sku, s => s.Shop, s => s.Shop.Wallet, s => s.Sin.Character);
            if (price == null)
                throw new BillingException("Персональное предложение не найдено");
            if (price.Confirmed)
                throw new Exception("Персональным предложением уже воспользовались");
            if (price.Sin.Character.Model != modelId)
                throw new Exception("Персональное предложение заведено на другого персонажа");
            var dateTill = price.DateCreated.AddMinutes(_settings.GetIntValue(SystemSettingsEnum.price_minutes));
            if (dateTill < DateTime.Now)
                throw new BillingException($"Персональное предложение больше не действительно, оно истекло {dateTill.ToString("HH:mm:ss")}");
            var sku = SkuAllowed(price.ShopId, price.SkuId);
            if (sku == null)
                throw new BillingException("Sku недоступно для продажи в данный момент");
            var sin = Get<SIN>(s => s.Id == price.SinId, s => s.Wallet, s => s.Character);
            if (sin.Wallet.Balance - price.FinalPrice < 0)
            {
                throw new BillingException("Недостаточно средств");
            }
            sku.Count--;
            var renta = new Renta
            {
                BasePrice = price.BasePrice,
                Sin = sin,
                CurrentScoring = price.CurrentScoring,
                Sku = price.Sku,
                DateCreated = DateTime.Now,
                Discount = price.Discount,
                ShopComission = price.ShopComission,
                ShopId = price.ShopId,
                HasQRWrite = BillingHelper.HasQrWrite(sku.Nomenklatura.Code),
                PriceId = priceId,
                Secret = sku.Nomenklatura.Secret,
                LifeStyle = sku.Nomenklatura.Lifestyle
            };
            Add(renta);
            price.Confirmed = true;
            Context.SaveChanges();
            ProcessByuScoring(sin, sku);
            ProcessRenta(renta, sin);

            EreminPushAdapter.SendNotification(modelId, "Покупка совершена", $"Вы купили {price.Sku.Name}");
            var dto = new RentaDto
            {
                HasQRWrite = renta.HasQRWrite,
                PriceId = priceId,
                RentaId = renta.Id,
                FinalPrice = price.FinalPrice
            };
            return dto;
        }

        private void ProcessByuScoring(SIN sin, Sku sku)
        {
            var type = sku.Nomenklatura.ProductType;
            if (type == null)
                throw new Exception("type not found");
            IScoringManager manager;
            switch (type.Alias)
            {
                case "Implant":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnImplantBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Food":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnFoodBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Weapon":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnWeaponBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Pill":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnPillBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;

                case "Magic":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnMagicBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                default:
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnOtherBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
            }
        }

        public PriceShopDto GetPriceByQR(int modelId, string qrid)
        {
            var qr = long.Parse(qrid);
            QRHelper.Parse(qr, out int skuId, out int shopId);
            if (skuId == 0 || shopId == 0)
            {
                throw new BillingException($"Ошибка распознования qr");
            }
            return GetPrice(modelId, shopId, skuId);
        }

        [BillingBlock]
        public PriceShopDto GetPrice(int modelId, int shopid, int skuid)
        {
            var sku = SkuAllowed(shopid, skuid);
            if (sku == null)
                throw new BillingException("sku недоступен для продажи");
            var shop = GetAsNoTracking<ShopWallet>(s => s.Id == shopid);
            var sin = GetSINByModelId(modelId, s => s.Scoring, s => s.Character);
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
                Add(corporation);
            }
            if (!string.IsNullOrEmpty(logoUrl))
                corporation.CorporationLogoUrl = logoUrl;
            if (!string.IsNullOrEmpty(name))
                corporation.Name = name;
            if (amount > 0)
                corporation.Wallet.Balance = amount;
            Context.SaveChanges();
            return corporation;
        }

        public ShopWallet CreateOrUpdateShopWallet(int shopId = 0, decimal amount = 0, string name = "default shop", int lifestyle = 1, int ownerId = 0)
        {
            ShopWallet shop = null;
            if (shopId > 0)
                shop = Get<ShopWallet>(w => w.Id == shopId, s => s.Wallet);
            var owner = GetSINByModelId(ownerId);
            if (owner == null)
                throw new BillingException("owner not found");

            if (shop == null)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Shop);
                shop = new ShopWallet
                {
                    Wallet = newWallet,
                    Id = shopId,
                    OwnerId = ownerId
                };
                Add(shop);
            }
            shop.Name = name;
            shop.Wallet.Balance = amount;
            shop.LifeStyle = (int)BillingHelper.GetLifestyle(lifestyle);
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
            if (nomenklaturas != null && nomenklaturas.Count > 0)
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
            if (skus != null && skus.Count > 0)
            {
                if (force)
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

        public BalanceDto GetBalance(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Scoring, s => s.Metatype);
            var balance = new BalanceDto
            {
                CharacterId = modelId,
                CurrentBalance = BillingHelper.RoundDown(sin.Wallet.Balance),
                CurrentScoring = sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative,
                SIN = sin.Sin,
                ForecastLifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.Balance).ToString(),
                LifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.Balance).ToString(),
                PersonName = sin.PersonName,
                Metatype = sin.Metatype?.Name ?? "неизвестно",
                Citizenship = sin.Citizenship,
                Nationality = sin.NationDisplay,
                Status = sin.Citizen_state,
                Nation = sin.Nation,
                Viza = sin.Viza,
                Pledgee = sin.Mortgagee
            };
            return balance;
        }

        public List<TransferDto> GetTransfers(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Character);
            if (sin == null)
                throw new BillingException("sin not found");
            var listFrom = GetList<Transfer>(t => t.WalletFromId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            var allList = new List<TransferDto>();
            var owner = GetWalletName(sin.Wallet, false);
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

        public string GetSinStringByCharacter(int modelId)
        {
            var sin = GetSINByModelId(modelId);
            if (sin == null)
                throw new Exception("sin not found");
            return sin.Sin;
        }

        public int GetModelIdBySinString(string sinString)
        {
            var sin = Get<SIN>(s => s.Sin == sinString);
            if (sin == null)
                throw new Exception("sin not found");
            return sin.Character.Model;
        }

        public ProductType CreateOrUpdateProductType(int id, string name, int discounttype = 1, int externalId = 0)
        {
            ProductType type = null;
            if (id > 0)
                type = Get<ProductType>(p => p.Id == id);
            if (type == null)
            {
                type = new ProductType();
                Add(type);
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
                Add(nomenklatura);
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
                Add(sku);
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
            Context.SaveChanges();
            return sku;
        }

        [BillingBlock]
        public Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

        [BillingBlock]
        public Transfer CreateTransferSINSIN(string modelid, string characterTo, decimal amount, string comment)
        {
            int imodelId;
            int icharacterTo;
            if (!int.TryParse(modelid, out imodelId) || imodelId == 0)
            {
                throw new BillingAuthException($"Ошибка авторизации {modelid}");
            }
            if (!int.TryParse(characterTo, out icharacterTo) || icharacterTo == 0)
            {
                throw new BillingAuthException($"Ошибка проверки получателя, должен быть инт");
            }
            return MakeTransferSINSIN(imodelId, icharacterTo, amount, comment);
        }

        public Transfer CreateTransferMIRSIN(string characterTo, decimal amount)
        {
            var from = GetMIR();
            int icharacterTo;
            if (!int.TryParse(characterTo, out icharacterTo) || icharacterTo == 0)
            {
                throw new BillingAuthException($"Ошибка проверки получателя, должен быть инт");
            }
            var to = GetSINByModelId(icharacterTo, s => s.Wallet);
            if (to == null)
            {
                throw new BillingException($"Не найден получатель");
            }
            var comment = "Перевод от международного банка";
            return MakeNewTransfer(from, to.Wallet, amount, comment);
        }

        [BillingBlock]
        public Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment)
        {
            var d1 = GetSINByModelId(characterFrom, s => s.Wallet);
            var d2 = GetSINByModelId(characterTo, s => s.Wallet);
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
            Context.SaveChanges();
            if (transfer != null)
            {
                EreminPushAdapter.SendNotification(characterTo, "Кошелек", $"Вам переведено денег {amount}");
            }
            return transfer;
        }

        public List<CharacterDto> GetCharacters()
        {
            var result = GetList<SIN>(s => s.InGame ?? false, s => s.Character).Select(s => new CharacterDto { PersonName = s.PersonName, ModelId = s.Character.Model.ToString() }).ToList();
            return result;
        }

        public List<TransferDto> GetTransfersByRenta(int rentaID)
        {
            var tranfers = GetListAsNoTracking<Transfer>(t => t.RentaId == rentaID).Select(s => CreateTransferDto(s, TransferType.Outcoming)).ToList();
            return tranfers;
        }

        #region private

        private void ProcessRenta(Renta renta, SIN sin)
        {
            var mir = GetMIR();
            ProcessRenta(renta, mir, sin);
            Context.SaveChanges();
        }

        private void ProcessRenta(Renta renta, Wallet mir, SIN sin)
        {
            if (renta == null)
            {
                throw new Exception("renta not found");
            }
            var shop = renta.Shop;
            if (shop == null || shop.Wallet == null)
            {
                shop = Get<ShopWallet>(s => s.Id == renta.ShopId, s => s.Wallet);
                if (shop == null)
                    throw new Exception("Shop not found");
            }
            var sku = renta.Sku;
            if (sku == null)
            {
                sku = Get<Sku>(s => s.Id == renta.SkuId, s => s.Corporation.Wallet);
            }
            var corporation = sku.Corporation;
            if (corporation == null || corporation.Wallet == null)
            {
                corporation = Get<CorporationWallet>(c => c.Id == sku.CorporationId, c => c.Wallet);
                if (corporation == null)
                    throw new Exception("corporation not found");
            }
            var character = sin.Character;
            if (character == null)
            {
                character = Get<Character>(c => c.Id == sin.CharacterId);
            }
            var walletFrom = sin.Wallet;
            if (walletFrom == null)
            {
                walletFrom = Get<Wallet>(w => w.Id == sin.WalletId);
            }
            var finalPrice = BillingHelper.GetFinalPrice(renta.BasePrice, renta.Discount, renta.CurrentScoring);
            var comission = BillingHelper.CalculateComission(renta.BasePrice, renta.ShopComission);
            //с кошелька списываем всегда
            MakeNewTransfer(walletFrom, mir, finalPrice, $"Рентный платеж: {sku.Name} в {shop.Name}", false, renta.Id);
            //если баланс положительный
            if (walletFrom.Balance > 0)
            {
                MakeNewTransfer(mir, corporation.Wallet, finalPrice - comission, $"Рентное начисление: {sku.Name} от {sin.PersonName} ({sin.Sin}) ", false, renta.Id);
                MakeNewTransfer(mir, shop.Wallet, comission, $"Рентное начисление: {sku.Name} в {shop.Name} от {sin.PersonName} ({sin.Sin})", false, renta.Id);
            }
            //EreminPushAdapter.SendNotification(character.Model, "Кошелек", $"Списание по рентному договору");
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
                discount = EreminService.GetDiscount(sin.Character.Model, GetDiscountTypeForSku(sku));
            }
            catch (Exception e)
            {
                discount = 0;
            }
            var currentScoring = sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative;
            var price = new Price
            {
                Sku = sku,
                Shop = shop,
                BasePrice = sku.Nomenklatura.BasePrice,
                CurrentScoring = currentScoring,
                DateCreated = DateTime.Now,
                Discount = discount,
                Sin = sin,
                ShopComission = shop.Commission,
                FinalPrice = BillingHelper.GetFinalPrice(sku.Nomenklatura.BasePrice, discount, currentScoring)
            };
            Add(price);
            Context.SaveChanges();
            return price;
        }

        private int ProcessRentas(SIN sin, Wallet mir)
        {
            var rentas = GetList<Renta>(r => r.SinId == sin.Id, r => r.Shop, r => r.Sku.Corporation);
            foreach (var renta in rentas)
            {
                try
                {
                    ProcessRenta(renta, mir, sin);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return rentas.Count;
        }
        #endregion
    }
}
