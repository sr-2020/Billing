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
    public interface IBillingManager
    {
        #region application
        Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment);
        Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);
        Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment);
        Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
        string GetSinStringByCharacter(int characterId);
        int GetCharacterIdBySin(string sinString);
        List<TransferDto> GetTransfers(int characterId);
        BalanceDto GetBalance(int characterId);
        List<RentaDto> GetRentas(int characterId);
        #endregion

        #region web
        Specialisation SetSpecialisation(int productType, int shop);
        void DropSpecialisation(int productType, int shop);
        ShopQR WriteQR(int qr, int shop, int sku);
        ShopQR WriteFreeQR(int shop, int sku);
        ShopQR CleanQR(int qr);
        void BreakContract(int corporation, int shop);
        Contract CreateContract(int corporation, int shop);
        Renta ConfirmRenta(int character, int priceId);
        List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled);
        List<SkuDto> GetSkusForShop(int shop);
        PriceDto GetPriceByQR(int character, int qr);
        PriceDto GetPrice(int character, int shop, int sku);
        List<ShopDto> GetShops();
        List<CorporationDto> GetCorps();
        List<ProductTypeDto> GetProductTypes();
        List<NomenklaturaDto> GetNomenklaturas(int producttype, int lifestyle);
        List<Contract> GetContrats(int shopid, int corporationId);

        #endregion

        #region admin

        Sku CreateOrUpdateSku(int id, int nomenklatura, int count, int corporation, string name, bool enabled);
        Nomenklatura CreateOrUpdateNomenklatura(int id, string name, string code, int producttype, int lifestyle, decimal baseprice, string description, string pictureurl);
        SIN CreateOrUpdatePhysicalWallet(int character, decimal balance);
        ProductType CreateOrUpdateProductType(int id, string name, int discounttype);
        CorporationWallet CreateOrUpdateCorporationWallet(int id, decimal amount, string name, string logoUrl);
        ShopWallet CreateOrUpdateShopWallet(int foreignKey, decimal amount, string name, int lifestyle);
        void DeleteCorporation(int corpid);
        void DeleteShop(int shopid);
        #endregion
    }

    public class BillingManager : BaseEntityRepository, IBillingManager
    {
        ISettingsManager _settings = IocContainer.Get<ISettingsManager>();
        public static string UrlNotFound = "https://www.clickon.ru/preview/original/pic/8781_logo.png";

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

        public ShopQR WriteQR(int qrid, int shop, int skuid)
        {
            //check allow
            var sku = SkuAllowed(shop, skuid);
            if (sku == null)
                throw new BillingException("Магазин не может продавать этот товар в данный момент");
            var qr = Get<ShopQR>(q => q.Id == qrid);
            if (qr == null)
                throw new Exception($"qr {qrid} not found");
            qr.ShopId = shop;
            qr.SkuId = skuid;
            Add(qr);
            Context.SaveChanges();
            return qr;
        }
        public ShopQR WriteFreeQR(int shop, int sku)
        {
            var qr = Get<ShopQR>(q => q.ShopId == null);
            if (qr == null)
                throw new BillingException("не найдено свободных QR");
            return WriteQR(qr.Id, shop, sku);
        }
        public ShopQR CleanQR(int qrid)
        {
            var qr = Get<ShopQR>(q => q.Id == qrid);
            if (qr == null)
                throw new Exception($"qr {qrid} not fount");
            qr.ShopId = null;
            qr.SkuId = null;
            Add(qr);
            Context.SaveChanges();
            return qr;
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

        public List<NomenklaturaDto> GetNomenklaturas(int producttype, int lifestyle)
        {
            var list = GetList<Nomenklatura>(n =>
                (n.ProductTypeId == producttype || producttype == 0)
                && (n.Lifestyle == lifestyle || lifestyle == 0), n => n.ProductType);
            return list.Select(s => new NomenklaturaDto(s)).ToList();
        }
        public List<Contract> GetContrats(int shopid, int corporationId)
        {
            var list = GetList<Contract>(c => (c.ShopId == shopid || shopid == 0) && (c.CorporationId == corporationId || corporationId == 0));
            return list;
        }

        public List<ProductTypeDto> GetProductTypes()
        {
            return GetList<ProductType>(p => true).Select(p =>
                new ProductTypeDto(p)).ToList();
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

        public List<ShopDto> GetShops()
        {
            return GetList<ShopWallet>(c => true, new string[] { "Wallet","Specialisations", "Specialisations.ProductType" }).Select(s =>
                    new ShopDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Comission = BillingHelper.GetComission(s.LifeStyle),
                        Lifestyle = s.LifeStyle,
                        Balance = s.Wallet.Balance,
                        Specialisations = CreateSpecialisationDto(s)
                    }).ToList();
        }

        public List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled)
        {
            var list = GetList<Sku>(s => (s.CorporationId == corporationId || corporationId == 0) && (s.NomenklaturaId == nomenklaturaId || nomenklaturaId == 0) && (s.Enabled == enabled || !enabled.HasValue), s => s.Corporation, s => s.Nomenklatura, s => s.Nomenklatura.ProductType);
            return list.Select(s => new SkuDto(s)).ToList();
        }

        public List<SkuDto> GetSkusForShop(int shop)
        {
            return GetSkuList(shop).Select(s => new SkuDto(s)).ToList();
        }

        public List<RentaDto> GetRentas(int characterId)
        {
            throw new NotImplementedException();
            //return GetList<Renta>(r => r.CharacterId == characterId, r => r.ProductType, r => r.Shop).Select(r =>
            //        new RentaDto
            //        {
            //            FinalPrice = r.FinalPrice,
            //            ProductType = r.ProductType.Name,
            //            Shop = r.Shop.Name
            //        }).ToList();
        }

        public Renta ConfirmRenta(int character, int priceId)
        {
            var price = Get<Price>(p => p.Id == priceId, p => p.Sku, s => s.Shop, s => s.Shop.Wallet);
            if (price == null)
                throw new BillingException("Персональное предложение не найдено");
            if (price.CharacterId != character)
                throw new Exception("Персональное предложение заведено на другого персонажа");
            var dateTill = price.DateCreated.AddMinutes(_settings.GetIntValue("price_minutes"));
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
            MakeNewTransfer(mir, sin.Wallet, price.FinalPrice, $"Первый платеж по ренте {price.Sku.Name} купленный в {price.Shop.Name}");
            var comission = price.BasePrice * (price.ShopComission / 100);
            MakeNewTransfer(price.Shop.Wallet, mir, comission, $"комиссия за продажу {price.Sku.Name} с син {sin.CharacterId}");
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
                ShopId = price.ShopId
            };
            Add(renta);
            Context.SaveChanges();
            return renta;
        }

        public PriceDto GetPriceByQR(int character, int qrid)
        {
            var qr = Get<ShopQR>(q => q.Id == qrid);
            if (qr == null)
                throw new Exception($"qr {qrid} not found");
            if (qr.ShopId == null || qr.SkuId == null)
                throw new BillingException("пустой qr");
            return GetPrice(character, qr.ShopId.Value, qr.SkuId.Value);
        }
        public PriceDto GetPrice(int character, int shopid, int skuid)
        {
            var sku = SkuAllowed(shopid, skuid);
            if (sku == null)
                throw new BillingException("sku недоступен для продажи в данный момент");
            var shop = Get<ShopWallet>(s => s.Id == shopid);
            var sin = GetSIN(character, s => s.Scoring);
            if (shop == null || sin == null)
                throw new Exception("some went wrong");
            var price = CreateNewPrice(sku, shop, sin);
            var dto = new PriceDto(price);
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
            if (listFrom != null)
                allList.AddRange(listFrom
                    .Select(s => CreateTransferDto(s, TransferType.Outcoming))
                    .ToList());
            var listTo = GetList<Transfer>(t => t.WalletToId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            if (listTo != null)
                allList.AddRange(listTo
                    .Select(s => CreateTransferDto(s, TransferType.Incoming))
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

        public ProductType CreateOrUpdateProductType(int id, string name, int discounttype = 1)
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
            if(!string.IsNullOrEmpty(name))
                type.Name = name;
            Add(type);
            Context.SaveChanges();
            return type;
        }

        public Nomenklatura CreateOrUpdateNomenklatura(int id, string name, string code, int producttypeid, int lifestyle, decimal baseprice, string description, string pictureurl)
        {
            Nomenklatura nomenklatura = null;
            if (id > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == id);
            if (nomenklatura == null)
            {
                nomenklatura = new Nomenklatura();
                nomenklatura.PictureUrl = UrlNotFound;
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
            ProductType producttype = null;
            if (producttypeid > 0)
                producttype = Get<ProductType>(p => p.Id == producttypeid);
            else
                producttype = Get<ProductType>(p => p.Id == nomenklatura.ProductTypeId);
            if (producttype == null)
                producttype = CreateOrUpdateProductType(producttypeid, "unknown producttype");
            nomenklatura.ProductTypeId = producttype.Id;
            if (lifestyle > 0)
                nomenklatura.Lifestyle = (int)BillingHelper.GetLifeStyleByBalance(lifestyle);
            nomenklatura.Lifestyle = (int)BillingHelper.GetLifeStyleByBalance(nomenklatura.Lifestyle);
            Add(nomenklatura);
            Context.SaveChanges();
            return nomenklatura;
        }

        public Sku CreateOrUpdateSku(int id, int nomenklaturaid, int count, int corporationid, string name, bool enabled)
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
                corporation = CreateOrUpdateCorporationWallet(corporationid);
            sku.CorporationId = corporation.Id;
            Nomenklatura nomenklatura = null;
            if (nomenklaturaid > 0)
                nomenklatura = Get<Nomenklatura>(n => n.Id == nomenklaturaid);
            else
                nomenklatura = Get<Nomenklatura>(n => n.Id == sku.NomenklaturaId);
            if (nomenklatura == null)
                nomenklatura = CreateOrUpdateNomenklatura(nomenklaturaid, "unknown nomenklatura", string.Empty, 0, 1, 0, "unknown nomenklatura", string.Empty);
            sku.NomenklaturaId = nomenklatura.Id;
            Add(sku);
            Context.SaveChanges();
            return sku;
        }

        public SIN CreateOrUpdatePhysicalWallet(int character = 0, decimal balance = 50)
        {
            if (character == 0)
                throw new BillingAuthException($"character {character} not found");
            var sin = Get<SIN>(s => s.CharacterId == character);
            if (sin == null)
            {
                sin = new SIN
                {
                    CharacterId = character
                };
            }
            Add(sin);
            sin.EVersion = _settings.GetValue("eversion");
            var wallet = CreateOrUpdateWallet(WalletTypes.Character, sin.WalletId, balance);
            var scoring = Get<Scoring>(s => s.Id == sin.ScoringId);
            if (scoring == null)
            {
                scoring = new Scoring
                {
                    CurrentScoring = 1
                };
                sin.Scoring = scoring;
            }
            Add(scoring);
            Context.SaveChanges();

            //TODO
            var categoryCalculates = GetList<ScoringCategoryCalculate>(c => c.ScoringId == scoring.Id);

            Context.SaveChanges();
            return sin;
        }

        public Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

        public Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

        public Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment)
        {
            throw new NotImplementedException();
        }

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
            return MakeNewTransfer(d1.Wallet, d2.Wallet, amount, comment, anon);
        }

        #region private

        private string _ownerName = "Владелец кошелька";

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
        private Sku SkuAllowed(int shop, int sku)
        {
            var skuList = GetSkuList(shop);
            return skuList.FirstOrDefault(s => s.Id == sku);
        }
        private List<Sku> GetSkuList(int shopId)
        {
            var skuids = ExecuteQuery<int>($"SELECT * FROM get_sku({shopId})");
            var result = GetList<Sku>(s=>skuids.Contains(s.Id), s => s.Corporation, s => s.Nomenklatura, s => s.Nomenklatura.ProductType);
            //TODO filter by contractlimit
            return result;
        }

        private Wallet CreateOrUpdateWallet(WalletTypes type, int id = 0, decimal amount = 0)
        {
            Wallet wallet;
            if (id > 0)
            {
                var inttype = (int)type;
                wallet = Get<Wallet>(w => w.Id == id && w.WalletType == inttype);
                if (wallet == null)
                    throw new Exception($"кошелек {id} type {type} не найден");
            }
            else
            {
                wallet = new Wallet();
                wallet.WalletType = (int)type;
                wallet.Balance = 0;
            }
            Add(wallet);
            Context.SaveChanges();
            if (amount > 0)
            {
                var mir = GetMIR();
                if (wallet.Balance > 0)
                    MakeNewTransfer(wallet, mir, wallet.Balance, "Сброс кошелька");
                wallet.Balance = 0;
                MakeNewTransfer(mir, wallet, amount, "Заведение кошелька");
            }
            return wallet;
        }

        private Wallet GetMIR()
        {
            var mir = Get<Wallet>(w => w.Id == GetMIRId() && w.WalletType == (int)WalletTypes.MIR);
            if (mir == null)
                throw new Exception("MIR not found");
            return mir;
        }

        private int GetMIRId()
        {
            return _settings.GetIntValue("MIR_ID");
        }

        private TransferDto CreateTransferDto(Transfer transfer, TransferType type)
        {
            return new TransferDto
            {
                Comment = transfer.Comment,
                TransferType = type.ToString(),
                Amount = transfer.Amount,
                NewBalance = type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime,
                From = type == TransferType.Incoming ? GetWalletName(transfer.WalletFrom) : _ownerName,
                To = type == TransferType.Incoming ? _ownerName : GetWalletName(transfer.WalletTo),
                Anonimous = transfer.Anonymous
            };
        }

        private string GetWalletName(Wallet wallet)
        {
            if (wallet == null)
                return string.Empty;
            switch (wallet.WalletType)
            {
                case (int)WalletTypes.Character:
                    var sin = Get<SIN>(s => s.WalletId == wallet.Id);
                    if (sin == null)
                        return string.Empty;
                    return $"Character {sin.CharacterId} {sin.PersonName} {sin.Sin}";
                case (int)WalletTypes.Corporation:
                    var corp = Get<CorporationWallet>(c => c.WalletId == wallet.Id);
                    if (corp == null)
                        return string.Empty;
                    return $"Corporation {corp.Id}";
                case (int)WalletTypes.Shop:
                    var shop = Get<ShopWallet>(c => c.WalletId == wallet.Id);
                    if (shop == null)
                        return string.Empty;
                    return $"Shop {shop.Id}";
                case (int)WalletTypes.MIR:
                    return "MIR";
                default:
                    return string.Empty;
            }
        }

        private SIN GetSIN(int characterId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.CharacterId == characterId, includes);
            if (sin == null)
            {
                sin = CreateOrUpdatePhysicalWallet(characterId);
            }
            return sin;
        }
        private Transfer MakeNewTransfer(Wallet wallet1, Wallet wallet2, decimal amount, string comment, bool anonymous = false)
        {
            if (wallet1 == null)
                throw new BillingException($"Нет кошелька отправителя");
            if (wallet2 == null)
                throw new BillingException($"Нет кошелька получателя");
            if (wallet1.Id == wallet2.Id)
                throw new BillingException($"Самому себе нельзя переводить.");
            if (wallet1.Balance < amount && wallet1.WalletType != (int)WalletTypes.MIR)
                throw new BillingException($"Денег нет, но вы держитесь");
            if (amount <= 0)
                throw new BillingException($"Нельзя перевести отрицательное значение");
            wallet1.Balance -= amount;
            Add(wallet1);
            wallet2.Balance += amount;
            Add(wallet2);
            Context.SaveChanges();
            var transfer = new Transfer
            {
                Amount = amount,
                Comment = comment,
                WalletFromId = wallet1.Id,
                WalletToId = wallet2.Id,
                NewBalanceFrom = wallet1.Balance,
                NewBalanceTo = wallet2.Balance,
                OperationTime = DateTime.Now,
                Anonymous = anonymous
            };
            Add(transfer);
            Context.SaveChanges();
            return transfer;
        }
        private DiscountType GetDiscountTypeForSku(Sku sku)
        {
            if (sku == null)
                throw new Exception("sku not found");
            var nomenklatura = sku.Nomenklatura;
            if (nomenklatura == null)
                nomenklatura = Get<Nomenklatura>(n => n.Id == sku.NomenklaturaId);
            if(nomenklatura == null)
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
                ShopComission = BillingHelper.GetComission(shop.LifeStyle),
                FinalPrice = BillingHelper.GetFinalPrice(sku.Nomenklatura.BasePrice, discount, sin.Scoring.CurrentScoring)
            };
            Add(price);
            Context.SaveChanges();
            return price;
        }
        #endregion
    }
}
