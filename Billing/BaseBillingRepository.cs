﻿using Billing.DTO;
using Core;
using Core.Exceptions;
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
    public interface IBaseBillingRepository : IBaseRepository
    {
        SIN GetSINByModelId(int modelId, params Expression<Func<SIN, object>>[] includes);
        SIN GetSINBySinText(string sinText, params Expression<Func<SIN, object>>[] includes);
        SIN BillingBlocked(int modelId, params Expression<Func<SIN, object>>[] includes);
        SIN BillingBlocked(string sinText, params Expression<Func<SIN, object>>[] includes);
    }

    public class BaseBillingRepository : BaseEntityRepository, IBaseBillingRepository
    {
        protected ISettingsManager _settings = IocContainer.Get<ISettingsManager>();
        private string BlockErrorMessage = $"В данный момент ведется пересчет рентных платежей, попробуйте повторить чуть позже";

        
        private string ErrorWalletName(string message)
        {
            Console.Error.WriteLine(message);
            return string.Empty;
        }

        protected string GetWalletName(Wallet wallet, bool anon, List<SIN> sinCache, List<ShopWallet> shopCache, string owner = "")
        {
            if (wallet == null)
                return ErrorWalletName($"Передан пустой кошелек");
            switch (wallet.WalletType)
            {
                case (int)WalletTypes.Character:
                    var sin = sinCache.FirstOrDefault(s => s.WalletId == wallet.Id);
                    if (sin == null)
                    {
                        return owner;
                    }
                    return BillingHelper.GetPassportName(sin.Passport, anon);
                case (int)WalletTypes.Corporation:
                    return ErrorWalletName($"Переводы корпорациям не реализованы wallet: {wallet.Id}");

                case (int)WalletTypes.Shop:
                    var shop = shopCache.FirstOrDefault(s => s.WalletId == wallet.Id);
                    if (shop == null)
                        return ErrorWalletName($"Не найден shop для wallet {wallet.Id}");
                    return $"{shop.Id} {shop.Name}";
                case (int)WalletTypes.MIR:
                    return "MIR";
                default:
                    return string.Empty;
            }
        }

        protected Renta CreateRenta(int modelId, int priceId, int beat, int count = 1)
        {
            var sin = BillingBlocked(modelId, s => s.Wallet, s => s.Character, s => s.Passport, s => s.Scoring);
            if (count == 0)
                count = 1;
            var price = Get<Price>(p => p.Id == priceId,
                p => p.Sku.Nomenklatura.Specialisation.ProductType,
                p => p.Sku.Corporation.Wallet,
                s => s.Shop.Wallet,
                s => s.Sin.Character);
            if (price == null)
                throw new BillingException("Персональное предложение не найдено");
            if (price.Confirmed)
                throw new Exception("Персональным предложением уже воспользовались");
            if (price.Sin.Character.Model != modelId)
                throw new Exception("Персональное предложение заведено на другого персонажа");
            var dateTill = price.DateCreated.AddMinutes(_settings.GetIntValue(SystemSettingsEnum.price_minutes));
            if (dateTill < DateTime.Now.ToUniversalTime())
                throw new BillingException($"Персональное предложение больше не действительно, оно истекло {dateTill:HH:mm:ss}");
            var allowed = SkuAllowed(price.ShopId, price.SkuId);
            if (allowed == null)
                throw new BillingException("Sku недоступно для продажи в данный момент");
            price.FinalPrice *= count;
            if (sin.Wallet.Balance - price.FinalPrice < 0)
            {
                throw new BillingException("Недостаточно средств");
            }

            price.Sku.Count -= count;
            var instantConsume = price.Sku.Nomenklatura.Specialisation.ProductType.InstantConsume;
            var anon = GetAnon(sin.Character.Model);
            var gmdescript = BillingHelper.GetGmDescription(sin.Passport, price.Sku, anon);
            var renta = new Renta
            {
                BasePrice = price.BasePrice,
                Sin = sin,
                CurrentScoring = price.CurrentScoring,
                Sku = price.Sku,
                DateCreated = DateTime.Now.ToUniversalTime(),
                Discount = price.Discount,
                ShopComission = price.ShopComission,
                ShopPrice = price.ShopPrice,
                ShopId = price.ShopId,
                Shop = price.Shop,
                HasQRWrite = instantConsume ? false : BillingHelper.HasQrWrite(price.Sku.Nomenklatura.Code),
                PriceId = priceId,
                Secret = gmdescript,
                LifeStyle = price.Sku.Nomenklatura.Lifestyle,
                Count = count,
                FullPrice = price.Sku.Nomenklatura.Specialisation.ProductType.Alias == ProductTypeEnum.Charity.ToString(),
                BeatId = beat
            };
            Add(renta);
            price.Confirmed = true;
            SaveContext();
            ProcessBuyScoring(sin, price.Sku, price.Shop);
            var mir = GetMIR();
            ProcessRenta(renta, mir, sin, true);
            SaveContext();
            if (instantConsume)
            {
                var erService = new EreminService();
                erService.ConsumeFood(renta.Id, (Lifestyles)renta.LifeStyle, modelId).GetAwaiter().GetResult();
            }
            return renta;
        }

        protected bool GetAnon(int modelId)
        {
            try
            {
                var erService = new EreminService();
                return erService.GetAnonimous(modelId);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Ошибка получения anonimous");
            }
            return false;
        }

        protected CorporationWallet GetMortagee(Passport passport)
        {
            return Get<CorporationWallet>(c => c.Alias == passport.Mortgagee);
        }

        protected Price CreateNewPrice(Sku sku, ShopWallet shop, SIN sin)
        {
            decimal modeldiscount;
            try
            {
                var eService = new EreminService();
                modeldiscount = eService.GetDiscount(sin.Character.Model, BillingHelper.GetDiscountType(sku.Nomenklatura.Specialisation.ProductType.DiscountType));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                modeldiscount = 1;
            }
            decimal discount = 1;
            if (sin.Passport.Mortgagee == sku.Corporation.Alias)
                discount *= 0.9m;
            discount *= modeldiscount;
            var currentScoring = sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative;
            if (currentScoring == 0)
            {
                currentScoring = 1;
            }
            var price = new Price
            {
                Sku = sku,
                Shop = shop,
                BasePrice = sku.Nomenklatura.BasePrice,
                CurrentScoring = currentScoring,
                DateCreated = DateTime.Now.ToUniversalTime(),
                Discount = discount,
                Sin = sin,
                ShopComission = BillingHelper.GetShopComission(shop.LifeStyle),
                ShopPrice = BillingHelper.GetShopPrice(sku)
            };
            price.FinalPrice = BillingHelper.GetFinalPrice(price);
            Add(price);
            SaveContext();
            return price;
        }

        /// <summary>
        /// TODO need caching
        /// </summary>
        protected TransferDto CreateTransferDto(Transfer transfer, TransferType type, List<SIN> sinCache, List<ShopWallet> shopCache, string owner = "владелец кошелька", bool overdraft = false)
        {
            bool anon = transfer.Anonymous;
            return new TransferDto
            {
                ModelId = "закрыто",
                Comment = transfer.Comment,
                TransferType = type.ToString(),
                Amount = BillingHelper.Round(transfer.Amount),
                NewBalance = overdraft ? 0 : type == TransferType.Incoming ? transfer.NewBalanceTo : transfer.NewBalanceFrom,
                OperationTime = transfer.OperationTime,
                From = GetWalletName(transfer.WalletFrom, anon, sinCache, shopCache, owner),
                To =  GetWalletName(transfer.WalletTo, anon, sinCache, shopCache, owner),
                Anonimous = transfer.Anonymous,
                Id = transfer.Id,
                Overdraft = transfer.Overdraft,
                RentaId = transfer.RentaId
            };
        }

        protected Transfer AddNewTransfer(Wallet walletFrom, Wallet walletTo, decimal amount, string comment, bool anonymous = false, int? rentaId = null, bool overdraft = false)
        {
            if (walletFrom == null)
                throw new BillingNotFoundException($"Нет кошелька отправителя");
            if (walletTo == null)
                throw new BillingNotFoundException($"Нет кошелька получателя");
            if (walletFrom.Id == walletTo.Id)
                throw new BillingException($"Самому себе нельзя переводить.");
            //баланса хватает, или один из кошельков MIR
            if (walletFrom.Balance < amount && walletFrom.WalletType != (int)WalletTypes.MIR && walletTo.WalletType != (int)WalletTypes.MIR)
                throw new BillingException($"Денег нет, но вы держитесь");
            if (amount < 0)
                throw new BillingException($"Нельзя перевести отрицательное значение");
            if (comment.Length > 255)
                comment = comment.Substring(0, 254);
            walletFrom.Balance -= amount;
            walletTo.Balance += amount;
            var transfer = new Transfer
            {
                Amount = amount,
                Comment = comment,
                WalletFromId = walletFrom.Id,
                WalletToId = walletTo.Id,
                NewBalanceFrom = walletFrom.Balance,
                NewBalanceTo = walletTo.Balance,
                OperationTime = DateTime.Now.ToUniversalTime(),
                Anonymous = anonymous,
                RentaId = rentaId,
                Overdraft = overdraft
            };
            Add(transfer);
            return transfer;
        }

        protected Wallet CreateOrUpdateWallet(WalletTypes type, int id = 0, decimal amount = 0)
        {
            Wallet wallet;
            if (id > 0)
            {
                var inttype = (int)type;
                wallet = Get<Wallet>(w => w.Id == id && w.WalletType == inttype);
                if (wallet == null)
                    throw new BillingNotFoundException($"кошелек {id} type {type} не найден");
            }
            else
            {
                wallet = new Wallet();
                wallet.WalletType = (int)type;
                Add(wallet);
            }
            wallet.Balance = amount;
            SaveContext();
            return wallet;
        }

        protected Wallet GetMIR()
        {
            var mir = Get<Wallet>(w => w.Id == GetMIRId() && w.WalletType == (int)WalletTypes.MIR);
            if (mir == null)
                throw new BillingNotFoundException("MIR not found");
            return mir;
        }

        public SIN BillingBlocked(int modelId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = GetSINByModelId(modelId, includes);
            if (sin?.Blocked ?? true)
            {
                throw new BillingException(BlockErrorMessage);
            }
            return sin;
        }

        public SIN BillingBlocked(string sinText, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = GetSINBySinText(sinText, includes);
            if (sin?.Blocked ?? true)
            {
                throw new BillingException(BlockErrorMessage);
            }
            return sin;
        }

        public SIN GetDisabledByModelId(int modelId, params Expression<Func<SIN, object>>[] includes)
        {
            if (modelId == 0)
                throw new BillingUnauthorizedException("Нужна авторизация");
            var sin = Get(s => s.Character.Model == modelId, includes);
            if (sin == null)
            {
                throw new BillingNotFoundException($"sin for modelId {modelId} not found");
            }
            return sin;
        }

        public SIN GetSINByModelId(int modelId, params Expression<Func<SIN, object>>[] includes)
        {
            if (modelId == 0)
                throw new BillingUnauthorizedException("Нужна авторизация");
            var sin = Get(s => s.Character.Model == modelId, includes);
            if (sin == null)
            {
                throw new BillingNotFoundException($"sin for modelId {modelId} not found");
            }
            if (!(sin.InGame ?? false))
            {
                throw new BillingException($"Персонаж {modelId} отключен");
            }
            return sin;
        }

        public SIN GetSINBySinText(string sinText, params Expression<Func<SIN, object>>[] includes)
        {
            sinText = sinText.ToUpper();
            var sin = Get(s => s.Passport.Sin == sinText, includes);
            if (sin == null)
            {
                throw new BillingNotFoundException($"sin for sinText {sinText} not found");
            }
            return sin;
        }

        protected Sku SkuAllowed(int shop, int sku, params Expression<Func<Sku, object>>[] includes)
        {
            var skuids = GetSkuIds(shop);
            if (skuids.Contains(sku))
                return Get(s => s.Id == sku, includes);
            throw new BillingNotFoundException($"sku {sku} для {shop} не найдено");
        }

        protected List<Sku> GetSkuList(int shopId, params Expression<Func<Sku, object>>[] includes)
        {
            var skuids = GetSkuIds(shopId);
            var result = GetList(s => skuids.Contains(s.Id), includes);
            return result;
        }

        /// <summary>
        /// НЕ ВЫПОЛНЯЕТСЯ SAVECONTEXT
        /// </summary>
        protected void ProcessRenta(Renta renta, Wallet mir, SIN sin, bool first = false)
        {
            if (renta?.Shop?.Wallet == null
                || renta?.Sku?.Corporation?.Wallet == null
                || sin?.Character == null
                || sin?.Wallet == null
                || renta?.Sku?.Nomenklatura == null)
            {
                throw new Exception("Ошибка загрузки моделей по ренте");
            }
            var finalPrice = BillingHelper.GetFinalPrice(renta);
            //если баланс положительный
            if (sin.Wallet.Balance > 0)
            {
                AddNewTransfer(sin.Wallet, mir, finalPrice, $"Рентный платеж: { renta.Sku.Name} в {renta.Shop.Name}", false, renta.Id, false);
                CloseOverdraft(renta, mir, sin, first);
            }
            else
            {
                AddNewTransfer(sin.Wallet, mir, finalPrice, $"Рентный платеж: {renta.Sku.Name} в {renta.Shop.Name}", false, renta.Id, true);
            }
        }
        protected void CloseOverdraft(Renta renta, Wallet mir, SIN sin, bool first = false)
        {
            decimal comission;
            if (renta.FullPrice)
            {
                comission = BillingHelper.GetFinalPrice(renta);
            }
            else
            {
                comission = BillingHelper.CalculateComission(renta);
            }
            //create KPI here
            renta.Sku.Corporation.CurrentKPI += renta.ShopPrice;
            if (first)
                renta.Sku.Corporation.SkuSold += renta.ShopPrice;
            //comission
            AddNewTransfer(mir, renta.Shop.Wallet, comission, $"Рентное начисление: {renta.Sku.Name} в {renta.Shop.Name} от {sin.Passport.PersonName}", false, renta.Id, false);
        }

        private void ProcessBuyScoring(SIN sin, Sku sku, ShopWallet shop)
        {
            var type = sku.Nomenklatura.Specialisation.ProductType;
            if (type == null)
                throw new Exception("type not found");
            var manager = IoC.IocContainer.Get<IScoringManager>();
            switch (type.Alias)
            {
                case "Implant":
                    manager.OnImplantBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Food":
                case "EdibleFood":
                    manager.OnFoodBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Weapon":
                    manager.OnWeaponBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Pill":
                    manager.OnPillBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Magic":
                    manager.OnMagicBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Insurance":
                    manager.OnInsuranceBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "Charity":
                    manager.OnCharityBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "drone":
                    manager.OnDroneBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                case "matrix":
                    manager.OnMatrixBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
                default:
                    manager.OnOtherBuy(sin, sku.Nomenklatura.Lifestyle);
                    break;
            }
            manager.OnShopLifestyle(sin, shop.LifeStyle);
        }

        private List<int> GetSkuIds(int shopId)
        {
            return ExecuteQuery<int>($"SELECT * FROM get_sku({shopId})");
        }

        private int GetMIRId()
        {
            return _settings.GetIntValue(SystemSettingsEnum.MIR_ID);
        }


    }
}
