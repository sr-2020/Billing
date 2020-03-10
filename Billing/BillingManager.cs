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
        #region in the game

        Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment);
        Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment);
        Transfer MakeTransferLegSIN(int legFrom, int sinTo, decimal amount, string comment);
        Transfer MakeTransferLegLeg(int legFrom, int legTo, decimal amount, string comment);
        string GetSinStringByCharacter(int characterId);
        int GetCharacterIdBySin(string sinString);
        PriceDto GetPrice(int productType, int corporation, int shop, int character, decimal basePrice, decimal shopComission = 0);
        Renta ConfirmRenta(int priceId);


        #endregion
        #region info

        List<TransferDto> GetTransfers(int characterId);
        BalanceDto GetBalance(int characterId);


        #endregion

        #region admin
        SIN CreateOrUpdatePhysicalWallet(int character, decimal balance);
        ProductType CreateOrUpdateProductType(string code, string name, string description);
        CorporationWallet CreateOrUpdateCorporationWallet(int foreignKey, decimal amount);
        ShopWallet CreateOrUpdateShopWallet(int foreignKey, decimal amount);
        //create legal wallet

        #endregion

    }

    public class BillingManager : BaseEntityRepository, IBillingManager
    {
        public Renta ConfirmRenta(int priceId)
        {
            var price = Get<Price>(p => p.Id == priceId);
            if (price == null)
                throw new BillingException("Персональное предложение не найдено");
            var dateTill = price.DateCreated.AddMinutes(IocContainer.Get<ISettingsManager>().GetIntValue("price_minutes"));
            if (dateTill < DateTime.Now)
                throw new BillingException($"Персональное предложение больше не действительно, оно истекло {dateTill.ToString("HH:mm:ss")}");
            
            throw new NotImplementedException("Заглушка");
        }

        public PriceDto GetPrice(int productType, int corporation, int shop, int character, decimal basePrice, decimal shopComission = 0)
        {
            var newPrice = CreateNewPrice(productType, corporation, shop, character, basePrice, shopComission);
            var dto = new PriceDto()
            {
                FinalPrice = (newPrice.BasePrice - (newPrice.BasePrice * (newPrice.Discount/100))) * newPrice.CurrentScoring,
                DateTill = newPrice.DateCreated.AddMinutes(IocContainer.Get<ISettingsManager>().GetIntValue("price_minutes")),
                PriceId = newPrice.Id
            };
            return dto;
        }

        public CorporationWallet CreateOrUpdateCorporationWallet(int foreignKey, decimal amount)
        {
            var db = Get<CorporationWallet>(w => w.Foreign == foreignKey);
            if (db == null)
            {
                var newWallet = new Wallet();
                Add(newWallet);
                db = new CorporationWallet
                {
                    Foreign = foreignKey,
                    Wallet = newWallet
                };
            }
            if (amount >= 0)
                db.Wallet.Balance = amount;
            Add(db);
            Context.SaveChanges();
            return db;
        }

        public ShopWallet CreateOrUpdateShopWallet(int foreignKey, decimal amount)
        {
            var db = Get<ShopWallet>(w => w.Foreign == foreignKey);
            if (db == null)
            {
                var newWallet = new Wallet();
                Add(newWallet);
                db = new ShopWallet
                {
                    Foreign = foreignKey,
                    Wallet = newWallet
                };
            }
            if (amount >= 0)
                db.Wallet.Balance = amount;
            Add(db);
            Context.SaveChanges();
            return db;
        }

        public BalanceDto GetBalance(int characterId)
        {
            var sin = GetSIN(characterId, s => s.Wallet, s => s.Scoring);
            if (sin == null)
                throw new BillingException("sin not found");
            var balance = new BalanceDto
            {
                CharacterId = characterId,
                CurrentBalance = sin.Wallet.Balance,
                CurrentScoring = sin.Scoring.CurrentScoring,
                SIN = sin.Sin,
                LifeStyle = LifeStyleHelper.GetLifeStyle(sin.Wallet.Balance).ToString()
            };
            return balance;
        }

        public List<TransferDto> GetTransfers(int characterId)
        {
            var sin = GetSIN(characterId);
            if (sin == null)
                throw new BillingException("sin not found");
            var listFrom = GetList<Transfer>(t => t.WalletFromId == sin.WalletId);
            var allList = new List<TransferDto>();
            if (listFrom != null)
                allList.AddRange(listFrom
                    .Select(s => CreateTransferDto(s, TransferType.Outcoming))
                    .ToList());
            var listTo = GetList<Transfer>(t => t.WalletToId == sin.WalletId);
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

        public ProductType CreateOrUpdateProductType(string code, string name, string description)
        {
            var type = Get<ProductType>(p => p.Code == code);
            if (type == null)
            {
                type = new ProductType();
                type.Code = code;
            }
            Add(type);
            type.Name = name;
            type.Description = description;
            Context.SaveChanges();
            return type;
        }

        public SIN CreateOrUpdatePhysicalWallet(int character, decimal balance)
        {
            var sin = GetSIN(character);

            if (sin == null)
            {
                sin = new SIN
                {
                    CharacterId = character,
                    ScoringId = 0,
                    WalletId = 0
                };
            }
            Add(sin);
            sin.EVersion = IocContainer.Get<ISettingsManager>().GetValue("eversion");
            var wallet = Get<Wallet>(w => w.Id == sin.WalletId);
            if (wallet == null)
            {
                wallet = new Wallet();
                sin.Wallet = wallet;
            }
            Add(wallet);
            if(balance >= 0)
                wallet.Balance = balance;
            wallet.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet.Balance);
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
            if (d1 == null)
            {
                d1 = CreateOrUpdatePhysicalWallet(characterFrom, 0);
            }
            var d2 = GetSIN(characterTo, s => s.Wallet);
            if (d2 == null)
            {
                d2 = CreateOrUpdatePhysicalWallet(characterTo, 0);
            }
            return MakeNewTransfer(d1.Wallet, d2.Wallet, amount, comment);
        }

        #region private
        private TransferDto CreateTransferDto(Transfer transfer, TransferType type)
        {
            return new TransferDto
            {
                Comment = transfer.Comment,
                TransferType = type.ToString(),
                Amount = transfer.Amount,
                NewBalance = type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime,
                From = transfer.WalletFrom.Id,
                To = transfer.WalletTo.Id
            };
        }
        private SIN GetSIN(int characterId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.CharacterId == characterId, includes);
            return sin;
        }
        private Transfer MakeNewTransfer(Wallet wallet1, Wallet wallet2, decimal amount, string comment)
        {
            if (wallet1 == null)
                throw new BillingException($"Админы нае***сь и не завели кошелек отправителю");
            if (wallet2 == null)
                throw new BillingException($"Админы нае***сь и не завели кошелек получателю");
            if (wallet1.Id == wallet2.Id)
                throw new BillingException($"Данная схема ухода от налогов невозможна, проще говоря сам себе не переведешь, никто не переведет.");
            if (wallet1.Balance < amount)
                throw new BillingException($"Денег нет, но вы держитесь");
            if (amount <= 0)
                throw new BillingException($"Ублюдок, мать твою, а ну иди сюда говно собачье, решил ко мне лезть? Ты, засранец вонючий, мать твою, а?");
            wallet1.Balance -= amount;
            wallet1.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet1.Balance);
            Add(wallet1);
            wallet2.Balance += amount;
            wallet2.Lifestyle = (int)LifeStyleHelper.GetLifeStyle(wallet2.Balance);
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
        private Price CreateNewPrice(int productType, int corporation, int shop, int character, decimal basePrice, decimal shopComission = 0)
        {
            if (productType == 0 || corporation == 0 || shop == 0 || character == 0)
                throw new BillingException($"Все параметры должны быть больше 0 - product_type:{productType}, corporation:{corporation}, shop:{shop}, character:{character}");
            //productType
            var pt = Get<ProductType>(p => p.Id == productType);
            if (pt == null)
                throw new BillingException("product_type не найден");
            //corporation
            var corpWallet = Get<CorporationWallet>(c => c.Foreign == corporation);
            if (corpWallet == null)
                corpWallet = CreateOrUpdateCorporationWallet(corporation, 0);
            //shop
            var shopWallet = Get<ShopWallet>(s => s.Foreign == shop);
            if (shopWallet == null)
                shopWallet = CreateOrUpdateShopWallet(shop, 0);
            //character
            var sin = GetSIN(character, s => s.Scoring);
            decimal discount = 0;
            try
            {
                discount = EreminService.GetDiscount(character);
            }
            catch (Exception e)
            {
                discount = 0;
            }

            var price = new Price
            {
                ProductTypeId = pt.Id,
                CorporationId = corpWallet.Id,
                ShopId = shopWallet.Id,
                BasePrice = basePrice,
                CurrentScoring = sin.Scoring.CurrentScoring,
                DateCreated = DateTime.Now,
                Discount = discount,
                SinId = sin.Id,
                ShopComission = shopComission
            };
            Add(price);
            Context.SaveChanges();
            return price;
        }
        #endregion
    }
}
