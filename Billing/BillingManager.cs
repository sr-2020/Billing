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

        Renta ConfirmRenta(int priceId);
        PriceDto GetPrice(int productTypeId, int corporationId, int shopId, int character, decimal basePrice, int comission);
        PriceDto GetPrice(int skuId, int character);
        List<ShopDto> GetShops();
        List<CorporationDto> GetCorps();
        List<ProductTypeDto> GetProductTypes();

        #endregion

        #region admin

        SIN CreateOrUpdatePhysicalWallet(int character, decimal balance);
        ProductType CreateOrUpdateProductType(string code, string name, string description, int lifestyle, int basePrice);
        CorporationWallet CreateOrUpdateCorporationWallet(int foreignKey, decimal amount, string name);
        ShopWallet CreateOrUpdateShopWallet(int foreignKey, decimal amount, string name, int comission);

        #endregion

    }

    public class BillingManager : BaseEntityRepository, IBillingManager
    {
        ISettingsManager _settings = IocContainer.Get<ISettingsManager>();

        public List<ProductTypeDto> GetProductTypes()
        {
            throw new NotImplementedException();
            //return GetList<ProductType>(p => true).Select(p =>
            //    new ProductTypeDto
            //        {
            //            Code = p.Code,
            //            Name = p.Name,
            //            Description = p.Description,
            //            LifeStyle = ((Lifestyles)p.Lifestyle).ToString()
            //        }).ToList();
        }

        public List<CorporationDto> GetCorps()
        {
            return GetList<CorporationWallet>(c => true).Select(c =>
                new CorporationDto
                {
                    Name = c.Name,
                    ForeignId = c.Foreign
                }).ToList();
        }

        public List<ShopDto> GetShops()
        {
            return GetList<ShopWallet>(c => true).Select(s =>
                new ShopDto
                {
                    Name = s.Name,
                    ForeignId = s.Foreign,
                    Comission = BillingHelper.GetComission(s.LifeStyle),
                    Lifestyle = s.LifeStyle
                }).ToList();
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

        public Renta ConfirmRenta(int priceId)
        {
            throw new NotImplementedException();
            //var price = Get<Price>(p => p.Id == priceId, p => p.ProductType, s => s.Shop);
            //if (price == null)
            //    throw new BillingException("Персональное предложение не найдено");
            //var dateTill = price.DateCreated.AddMinutes(_settings.GetIntValue("price_minutes"));
            //if (dateTill < DateTime.Now)
            //    throw new BillingException($"Персональное предложение больше не действительно, оно истекло {dateTill.ToString("HH:mm:ss")}");
            //var sin = GetSIN(price.CharacterId, s => s.Wallet);
            //if (sin.Wallet.Balance - price.FinalPrice < 0)
            //{
            //    throw new BillingException("Недостаточно средств");
            //}
            //var mir = GetMIR();
            //MakeNewTransfer(mir, sin.Wallet, price.FinalPrice, $"Первый платеж по ренте {price.ProductType.Name} купленный в {price.Shop.Name}");
            //var renta = new Renta
            //{
            //    BasePrice = price.BasePrice,
            //    CharacterId = sin.CharacterId,
            //    CorporationId = price.CorporationId,
            //    CurrentScoring = price.CurrentScoring,
            //    DateCreated = DateTime.Now,
            //    Discount = price.Discount,
            //    FinalPrice = price.FinalPrice,
            //    ProductTypeId = price.ProductTypeId,
            //    ShopComission = price.ShopComission,
            //    ShopId = price.ShopId
            //};
            //return renta;
        }

        public PriceDto GetPrice(int skuId, int character)
        {
            throw new NotImplementedException();
        }

        public PriceDto GetPrice(int productTypeId, int corporationId, int shopId, int character, decimal basePrice, int comission)
        {
            var productType = Get<ProductType>(t => t.Id == productTypeId);
            if (productType == null)
                throw new Exception($"producttype {productTypeId} not found");
            var corporation = Get<CorporationWallet>(c => c.Foreign == corporationId);
            if (corporation == null)
                corporation = CreateOrUpdateCorporationWallet();
            var shop = Get<ShopWallet>(s => s.Foreign == shopId);
            if (shop == null)
                shop = CreateOrUpdateShopWallet();
            var sin = GetSIN(character);
            var newPrice = CreateNewPrice(productType, corporation, shop, sin);
            var dto = new PriceDto()
            {
                FinalPrice = newPrice.FinalPrice,
                DateTill = newPrice.DateCreated.AddMinutes(_settings.GetIntValue("price_minutes")),
                PriceId = newPrice.Id
            };
            return dto;
        }

        public CorporationWallet CreateOrUpdateCorporationWallet(int foreignKey = 0, decimal amount = 0, string name = "default corporation")
        {
            var corporation = Get<CorporationWallet>(w => w.Foreign == foreignKey, c => c.Wallet);
            if (corporation == null)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Corporation);
                corporation = new CorporationWallet
                {
                    Foreign = foreignKey,
                    Wallet = newWallet
                };
            }
            corporation.Name = name;
            corporation.Wallet.Balance = amount;
            Add(corporation);
            Context.SaveChanges();
            if (corporation.Foreign == 0)
            {
                corporation.Foreign = corporation.Id;
                Add(corporation);
                Context.SaveChanges();
            }
            return corporation;
        }

        public ShopWallet CreateOrUpdateShopWallet(int foreignKey = 0, decimal amount = 0, string name = "default shop", int comission = 1)
        {
            var shop = Get<ShopWallet>(w => w.Foreign == foreignKey, s => s.Wallet);
            if (shop == null)
            {
                var newWallet = CreateOrUpdateWallet(WalletTypes.Shop);
                shop = new ShopWallet
                {
                    Foreign = foreignKey,
                    Wallet = newWallet,
                    LifeStyle = (int)Lifestyles.Wood
                };
            }
            shop.Name = name;
            shop.Wallet.Balance = amount;
            if(Enum.IsDefined(typeof(Lifestyles), shop.LifeStyle))
                shop.LifeStyle = shop.LifeStyle;
            else
                shop.LifeStyle = (int)Lifestyles.Wood;
            Add(shop);
            Context.SaveChanges();
            return shop;
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
                ForecastLifeStyle = BillingHelper.GetLifeStyle(sin.Wallet.Balance).ToString(),
                LifeStyle = BillingHelper.GetLifeStyle(sin.Wallet.Balance).ToString()
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

        public ProductType CreateOrUpdateProductType(string code, string name, string description, int lifestyle, int basePrice)
        {
            throw new NotImplementedException();
            //var type = Get<ProductType>(p => p.Code == code);
            //if (!Enum.IsDefined(typeof(Lifestyles), lifestyle))
            //    throw new BillingException($"lifestyle must be valid from 1 to 6, recieved {lifestyle}");

            //if (type == null)
            //{
            //    type = new ProductType
            //    {
            //        Code = code
            //    };
            //}
            //type.BasePrice = basePrice;
            //type.Name = name;
            //type.Description = description;
            //type.Lifestyle = lifestyle;
            //Add(type);
            //Context.SaveChanges();
            //return type;
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
            return MakeNewTransfer(d1.Wallet, d2.Wallet, amount, comment);
        }

        #region private

        private string _ownerName = "Владелец кошелька";

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
                To = type == TransferType.Incoming ? _ownerName : GetWalletName(transfer.WalletTo)
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
                    return $"Corporation {corp.Foreign}";
                case (int)WalletTypes.Shop:
                    var shop = Get<ShopWallet>(c => c.WalletId == wallet.Id);
                    if (shop == null)
                        return string.Empty;
                    return $"Shop {shop.Foreign}";
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
        private Transfer MakeNewTransfer(Wallet wallet1, Wallet wallet2, decimal amount, string comment)
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
                OperationTime = DateTime.Now
            };
            Add(transfer);
            Context.SaveChanges();
            return transfer;
        }
        private Price CreateNewPrice(ProductType productType, CorporationWallet corporation, ShopWallet shop, SIN sin)
        {
            throw new NotImplementedException();
            //decimal discount;
            //try
            //{
            //    discount = EreminService.GetDiscount(sin.CharacterId);
            //}
            //catch (Exception e)
            //{
            //    discount = 0;
            //}
            
            //var price = new Price
            //{
            //    ProductTypeId = productType.Id,
            //    CorporationId = corporation.Id,
            //    ShopId = shop.Id,
            //    BasePrice = productType.BasePrice,
            //    CurrentScoring = sin.Scoring.CurrentScoring,
            //    DateCreated = DateTime.Now,
            //    Discount = discount,
            //    CharacterId = sin.CharacterId,
            //    ShopComission = shop.Comission,
            //    FinalPrice = BillingHelper.GetFinalPrice(productType.BasePrice, discount, sin.Scoring.CurrentScoring)
            //};
            //Add(price);
            //Context.SaveChanges();
            //return price;
        }
        #endregion
    }
}
