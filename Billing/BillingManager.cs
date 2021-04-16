﻿using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using InternalServices;
using InternalServices.EreminModel;
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
        TransferSum GetTransfers(int modelId);
        BalanceDtoOld GetBalanceOld(int modelId);
        BalanceDto GetBalance(int modelId);
        RentaSumDto GetRentas(int modelId);
        #endregion

        #region web
        PriceShopDto GetPriceByQR(int character, string qrid);
        PriceShopDto GetPrice(int modelId, int shop, int sku);
        void BreakContract(int corporation, int shop);
        Contract CreateContract(int corporation, int shop);
        RentaDto ConfirmRenta(int modelId, int priceId);
        List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled, int id = -1);
        ProductType GetExtProductType(string name);
        Nomenklatura GetExtNomenklatura(string name);
        Sku GetExtSku(string name);
        List<Contract> GetContrats(int shopid, int corporationId);
        #endregion

        #region jobs

        List<SIN> GetActiveSins(params Expression<Func<SIN, object>>[] includes);
        void ProcessCharacterBeat(int sinId, decimal karmaCount, bool dividents1, bool dividents2, bool dividents3);

        #endregion

        #region admin

        void LetMePay(string modelId, string rentaId);
        void Rerent(string rentaId);
        void LetHimPay(string modelId, string targetId, string rentaId);
        List<SIN> GetSinsInGame();
        List<CharacterDto> GetCharactersInGame();
        List<TransferDto> GetTransfersByRenta(int rentaID);

        #endregion
    }

    public class BillingManager : AdminManager, IBillingManager
    {

        protected int CURRENTGAME = 2;

        public List<SIN> GetActiveSins(params Expression<Func<SIN, object>>[] includes)
        {
            var currentGame = 2;
            //var sins = GetListAsNoTracking(s => s.Character.Model == 44043, includes);
            var sins = GetListAsNoTracking(s => s.InGame ?? false && s.Character.Game == currentGame, includes);
            return sins;
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

        public List<Contract> GetContrats(int shopid, int corporationId)
        {
            var list = GetListAsNoTracking<Contract>(c => (c.ShopId == shopid || shopid == 0) && (c.CorporationId == corporationId || corporationId == 0));
            return list;
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
            var list = GetSkus(s => (s.CorporationId == corporationId || corporationId == 0)
                && (s.NomenklaturaId == nomenklaturaId || nomenklaturaId == 0)
                && (s.Enabled == enabled || !enabled.HasValue)
                && (s.Id == id || id == -1));
            return list;
        }

        public RentaSumDto GetRentas(int modelId)
        {
            var sum = new RentaSumDto();
            var list = GetListAsNoTracking<Renta>(r => r.Sin.Character.Model == modelId,
                r => r.Sku.Nomenklatura.Specialisation.ProductType,
                r => r.Sku.Corporation,
                r => r.Shop,
                r => r.Sin.Character)
                .Select(r =>
                    new RentaDto
                    {
                        ModelId = modelId.ToString(),
                        CharacterName = r.Sin.PersonName,
                        FinalPrice = Math.Round(BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring), 2),
                        ProductType = r.Sku.Nomenklatura.Specialisation.Name,
                        Shop = r.Shop.Name,
                        NomenklaturaName = r.Sku.Nomenklatura.Name,
                        SkuName = r.Sku.Name,
                        Corporation = r.Sku.Corporation.Name,
                        RentaId = r.Id,
                        HasQRWrite = r.HasQRWrite,
                        QRRecorded = r.QRRecorded,
                        DateCreated = r.DateCreated
                    }).ToList();
            sum.Rentas = list;
            sum.Sum = list.Sum(r => r.FinalPrice);
            return sum;

        }

        public RentaDto ConfirmRenta(int modelId, int priceId)
        {
            BillingHelper.BillingBlocked(modelId);
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
            if (dateTill < DateTime.Now)
                throw new BillingException($"Персональное предложение больше не действительно, оно истекло {dateTill.ToString("HH:mm:ss")}");
            var allowed = SkuAllowed(price.ShopId, price.SkuId);
            if (allowed == null)
                throw new BillingException("Sku недоступно для продажи в данный момент");
            var sin = Get<SIN>(s => s.Id == price.SinId, s => s.Wallet, s => s.Character);
            if (sin.Wallet.Balance - price.FinalPrice < 0)
            {
                throw new BillingException("Недостаточно средств");
            }
            price.Sku.Count--;
            var instantConsume = price.Sku.Nomenklatura.Specialisation.ProductType.InstantConsume;
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
                Shop = price.Shop,
                HasQRWrite = instantConsume ? false : BillingHelper.HasQrWrite(price.Sku.Nomenklatura.Code),
                PriceId = priceId,
                Secret = price.Sku.Nomenklatura.Secret,
                LifeStyle = price.Sku.Nomenklatura.Lifestyle
            };
            Add(renta);
            price.Confirmed = true;
            SaveContext();
            ProcessBuyScoring(sin, price.Sku);
            var mir = GetMIR();
            ProcessRenta(renta, mir, sin);
            SaveContext();
            if(instantConsume)
            {
                var erService = new EreminService();
                erService.ConsumeFood(renta.Id, (Lifestyles)renta.LifeStyle, modelId);
            }
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

        public void ProcessCharacterBeat(int sinId, decimal karmaCount, bool dividents1, bool dividents2, bool dividents3)
        {
            var sin = BlockCharacter(sinId);
            SaveContext();
            var mir = GetMIR();
            decimal sum = 0;
            //ability
            if (dividents1)
            {
                var dk1 = _settings.GetDecimalValue(SystemSettingsEnum.dividents1_k);
                AddNewTransfer(mir, sin.Wallet, dk1, "Дивиденды *");
                sum += dk1;
            }
            if (dividents2)
            {
                var dk2 = _settings.GetDecimalValue(SystemSettingsEnum.dividents2_k);
                AddNewTransfer(mir, sin.Wallet, dk2, "Дивиденды **");
                sum += dk2;
            }
            if (dividents3)
            {
                var dk3 = _settings.GetDecimalValue(SystemSettingsEnum.dividents3_k);
                AddNewTransfer(mir, sin.Wallet, dk3, "Дивиденды ***");
                sum += dk3;
            }
            //karma
            if (karmaCount > 0)
            {
                var k = _settings.GetDecimalValue(SystemSettingsEnum.karma_k);
                var karmasum = k * karmaCount;
                sum += karmasum;
                AddNewTransfer(mir, sin.Wallet, karmasum, "пассивный доход");
            }
            //rentas
            var rentas = GetList<Renta>(r => r.SinId == sin.Id, r => r.Shop.Wallet, r => r.Sku.Corporation.Wallet);
            foreach (var renta in rentas)
            {
                ProcessRenta(renta, mir, sin);
            }
            //scoring

            //forecast
            sum -= rentas.Sum(r => BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring));
            sin.Wallet.IncomeOutcome = sin.Wallet.Balance - (sum * 3);
            UnblockCharacter(sin);
            SaveContext();
        }

        /// <summary>
        /// НЕ ВЫПОЛНЯЕТСЯ SAVECONTEXT
        /// </summary>
        private void ProcessRenta(Renta renta, Wallet mir, SIN sin)
        {
            if (renta?.Shop?.Wallet == null
                || renta?.Sku?.Corporation?.Wallet == null
                || sin?.Character == null
                || sin?.Wallet == null)
            {
                throw new Exception("Ошибка загрузки моделей по ренте");
            }
            var finalPrice = BillingHelper.GetFinalPrice(renta.BasePrice, renta.Discount, renta.CurrentScoring);
            //если баланс положительный
            if (sin.Wallet.Balance > 0)
            {
                AddNewTransfer(sin.Wallet, mir, finalPrice, $"Рентный платеж: { renta.Sku.Name} в {renta.Shop.Name}", false, renta.Id, false);
                CloseOverdraft(renta, mir, sin);
                //close overdraft here
                var allOverdrafts = GetList<Transfer>(t => t.Overdraft && t.WalletFromId == sin.Wallet.Id && t.RentaId > 0);
                foreach (var overdraft in allOverdrafts)
                {
                    overdraft.Overdraft = false;
                    var closingRenta = Get<Renta>(r => r.Id == overdraft.RentaId);
                    CloseOverdraft(renta, mir, sin);
                }
            }
            else
            {
                AddNewTransfer(sin.Wallet, mir, finalPrice, $"Рентный платеж: {renta.Sku.Name} в {renta.Shop.Name}", false, renta.Id, true);
            }
        }

        private void CloseOverdraft(Renta renta, Wallet mir, SIN sin)
        {
            var comission = BillingHelper.CalculateComission(renta.BasePrice, renta.ShopComission);
            //create KPI here
            renta.Sku.Corporation.CurrentKPI += renta.BasePrice;
            //comission
            AddNewTransfer(mir, renta.Shop.Wallet, comission, $"Рентное начисление: {renta.Sku.Name} в {renta.Shop.Name} от {sin.PersonName} ({sin.Sin})", false, renta.Id, false);
        }

        private void ProcessBuyScoring(SIN sin, Sku sku)
        {
            var type = sku.Nomenklatura.Specialisation.ProductType;
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
                case "Insurance":
                    manager = IoC.IocContainer.Get<IScoringManager>();
                    manager.OnInsuranceBuy(sin, sku.Nomenklatura.Lifestyle);
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

        public PriceShopDto GetPrice(int modelId, int shopid, int skuid)
        {
            BillingHelper.BillingBlocked(modelId);
            var sku = SkuAllowed(shopid, skuid);
            if (sku == null)
                throw new BillingException("sku недоступен для продажи");
            var shop = GetAsNoTracking<ShopWallet>(s => s.Id == shopid);
            var sin = GetSINByModelId(modelId, s => s.Scoring, s => s.Character);
            if (shop == null || sin == null)
                throw new Exception("some went wrong");
            var price = CreateNewPrice(sku, shop, sin);
            var dto = new PriceShopDto(new PriceDto(price, true), sku.Corporation);
            return dto;
        }

        public BalanceDtoOld GetBalanceOld(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Scoring, s => s.Metatype);
            var balance = new BalanceDtoOld
            {
                ModelId = modelId,
                CurrentBalance = BillingHelper.RoundDown(sin.Wallet.Balance),
                CurrentScoring = sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative,
                SIN = sin.Sin,
                ForecastLifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.Balance).ToString(),
                LifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.Balance).ToString(),
                PersonName = sin.PersonName,
                Metatype = sin.Metatype?.Name ?? "неизвестно",
                Citizenship = sin.Citizenship ?? "неизвестно",
                Nationality = sin.NationDisplay ?? "неизвестно",
                Status = sin.Citizen_state ?? "неизвестно",
                Nation = sin.Nation ?? "неизвестно",
                Viza = sin.Viza ?? "неизвестно",
                Pledgee = sin.Mortgagee ?? "неизвестно"
            };
            return balance;
        }

        public BalanceDto GetBalance(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Scoring, s => s.Metatype);
            var inss = ProductTypeEnum.Insurance.ToString();
            var insur = GetList<Renta>(r => r.Sku.Nomenklatura.Specialisation.ProductType.Alias == inss, r => r.Sku)
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();
            var lics = ProductTypeEnum.Insurance.ToString();
            var licences = GetList<Renta>(r => r.Sku.Nomenklatura.Specialisation.ProductType.Alias == lics, r => r.Sku.Nomenklatura)
                .OrderByDescending(r => r.DateCreated)
                .GroupBy(l => l.Sku.Nomenklatura.SpecialisationId)
                .Select(g => g.FirstOrDefault()?.Sku?.Name)
                .ToList();
            var balance = new BalanceDto
            {
                ModelId = modelId,
                CurrentBalance = BillingHelper.RoundDown(sin.Wallet.Balance),
                CurrentScoring = Math.Round(sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative, 2),
                SIN = sin.Sin,
                LifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.Balance).ToString(),
                ForecastLifeStyle = BillingHelper.GetLifeStyleByBalance(sin.Wallet.IncomeOutcome).ToString(),
                PersonName = sin.PersonName,
                Metatype = sin.Metatype?.Name ?? "неизвестно",
                Citizenship = sin.Citizenship ?? "неизвестно",
                Nationality = sin.NationDisplay ?? "неизвестно",
                Status = sin.Citizen_state ?? "неизвестно",
                Nation = sin.Nation ?? "неизвестно",
                Viza = sin.Viza ?? "неизвестно",
                Pledgee = sin.Mortgagee ?? "неизвестно",
                Insurance = insur?.Sku?.Name ?? "нет страховки",
                Licenses = licences
            };
            return balance;
        }

        public TransferSum GetTransfers(int modelId)
        {
            var result = new TransferSum();
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Character);
            if (sin == null)
                throw new BillingException("sin not found");
            var listFrom = GetList<Transfer>(t => t.WalletFromId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            var allList = new List<TransferDto>();
            var owner = GetWalletName(sin.Wallet, false);
            if (listFrom != null)
                allList.AddRange(listFrom
                    .Select(s => CreateTransferDto(s, TransferType.Outcoming, modelId, owner))
                    .ToList());
            var listTo = GetList<Transfer>(t => t.WalletToId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            if (listTo != null)
                allList.AddRange(listTo
                    .Select(s => CreateTransferDto(s, TransferType.Incoming, modelId, owner))
                    .ToList());
            result.Transfers = allList.OrderBy(t => t.OperationTime).ToList();
            return result;
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

        public Transfer MakeTransferSINLeg(int sinFrom, int legTo, decimal amount, string comment)
        {
            //BillingHelper.BillingBlocked(modelId);
            throw new NotImplementedException();
        }

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
            BillingHelper.BillingBlocked(imodelId);
            BillingHelper.BillingBlocked(icharacterTo);
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
            return AddNewTransfer(from, to.Wallet, amount, comment);
        }

        public Transfer MakeTransferSINSIN(int characterFrom, int characterTo, decimal amount, string comment)
        {
            BillingHelper.BillingBlocked(characterFrom);
            BillingHelper.BillingBlocked(characterTo);
            var d1 = GetSINByModelId(characterFrom, s => s.Wallet);
            var d2 = GetSINByModelId(characterTo, s => s.Wallet);
            var anon = false;
            try
            {
                var erService = new EreminService();
                var anonFrom = erService.GetAnonimous(characterFrom);
                var anonto = erService.GetAnonimous(characterTo);
                anon = anonFrom || anonto;
            }
            catch (Exception e)
            {

            }
            var transfer = AddNewTransfer(d1.Wallet, d2.Wallet, amount, comment, anon);
            Context.SaveChanges();
            if (transfer != null)
            {
                EreminPushAdapter.SendNotification(characterTo, "Кошелек", $"Вам переведено денег {amount}");
            }
            return transfer;
        }

        public void LetMePay(string modelId, string rentaId)
        {
            if (string.IsNullOrEmpty(modelId) || string.IsNullOrEmpty(rentaId))
            {
                Console.Error.WriteLine($"Ошибка LetMePay modelId {modelId}, rentaId {rentaId}");
                return;
            }
            var modelIdInt = 0;
            if (!int.TryParse(modelId, out modelIdInt))
            {
                Console.Error.WriteLine($"Ошибка LetMePay modelId {modelId}");
                return;
            }
            if (modelIdInt == 0)
            {
                Console.Error.WriteLine($"Ошибка LetMePay modelId {modelId}");
                return;
            }
            var sin = GetSINByModelId(modelIdInt, s => s.Scoring);
            var rentaIdint = 0;
            if (!int.TryParse(rentaId, out rentaIdint))
            {
                Console.Error.WriteLine($"Ошибка LetMePay rentaId {rentaId}");
                EreminPushAdapter.SendNotification(modelIdInt, "Давай я заплачу", $"Ошибка получения ренты {rentaIdint}");
            }
            var renta = Get<Renta>(r => r.Id == rentaIdint);
            if (renta == null)
            {
                Console.Error.WriteLine($"Ошибка LetMePay rentaId {rentaId}");
                EreminPushAdapter.SendNotification(modelIdInt, "Давай я заплачу", "Ошибка получения ренты");
            }
            renta.SinId = sin.Id;
            renta.CurrentScoring = sin.Scoring.CurerentRelative + sin.Scoring.CurrentFix;
            Context.SaveChanges();
            EreminPushAdapter.SendNotification(modelIdInt, "Давай я заплачу", "Рента переоформлена");
        }

        public void Rerent(string rentaId)
        {
            var rentaIdint = 0;
            if (!int.TryParse(rentaId, out rentaIdint))
            {
                Console.Error.WriteLine($"Ошибка Rerent rentaId {rentaId}");
            }
            var renta = Get<Renta>(r => r.Id == rentaIdint, r => r.Sin.Scoring, r => r.Sin.Character);
            if (renta == null)
            {
                Console.Error.WriteLine($"Ошибка Rerent rentaId {rentaId}");
            }
            renta.CurrentScoring = renta.Sin.Scoring.CurerentRelative + renta.Sin.Scoring.CurrentFix;
            Context.SaveChanges();
            EreminPushAdapter.SendNotification(renta.Sin.Character.Model, "Давай он заплатит", "Рента переоформлена");
        }

        public void LetHimPay(string modelId, string targetId, string rentaId)
        {
            if (string.IsNullOrEmpty(modelId) || string.IsNullOrEmpty(rentaId))
            {
                Console.Error.WriteLine($"Ошибка LetHimPay modelId {modelId}, rentaId {rentaId}");
                return;
            }
            var modelIdInt = 0;
            if (!int.TryParse(modelId, out modelIdInt))
            {
                Console.Error.WriteLine($"Ошибка LetHimPay modelId {modelId}");
                return;
            }
            if (modelIdInt == 0)
            {
                Console.Error.WriteLine($"Ошибка LetHimPay modelId {modelId}");
                return;
            }
            var targetIdInt = 0;
            if (!int.TryParse(targetId, out targetIdInt))
            {
                Console.Error.WriteLine($"Ошибка LetHimPay targetId {targetId}");
                return;
            }
            if (targetIdInt == 0)
            {
                Console.Error.WriteLine($"Ошибка LetHimPay targetIdInt {targetId}");
                return;
            }
            var sin = GetSINByModelId(targetIdInt, s => s.Scoring);
            var rentaIdint = 0;
            if (!int.TryParse(rentaId, out rentaIdint))
            {
                Console.Error.WriteLine($"Ошибка LetHimPay rentaId {rentaId}");
                EreminPushAdapter.SendNotification(modelIdInt, "Давай он заплатит", $"Ошибка получения ренты {rentaIdint}");
            }
            var renta = Get<Renta>(r => r.Id == rentaIdint);
            if (renta == null)
            {
                Console.Error.WriteLine($"Ошибка LetHimPay rentaId {rentaId}");
                EreminPushAdapter.SendNotification(modelIdInt, "Давай он заплатит", "Ошибка получения ренты");
            }
            renta.SinId = sin.Id;
            renta.CurrentScoring = sin.Scoring.CurerentRelative + sin.Scoring.CurrentFix;
            Context.SaveChanges();
            EreminPushAdapter.SendNotification(modelIdInt, "Давай он заплатит", "Рента переоформлена");
        }

        public List<SIN> GetSinsInGame()
        {
            return GetSinsInGame(s => s.Character, s => s.Wallet, s => s.Scoring);
        }

        public List<CharacterDto> GetCharactersInGame()
        {
            var result = GetSinsInGame(s => s.Character).Select(s => new CharacterDto { PersonName = s.PersonName, ModelId = s.Character.Model.ToString() }).ToList();
            return result;
        }

        public List<TransferDto> GetTransfersByRenta(int rentaID)
        {
            var tranfers = GetListAsNoTracking<Transfer>(t => t.RentaId == rentaID).Select(s => CreateTransferDto(s, TransferType.Outcoming)).ToList();
            return tranfers;
        }

        #region private

        private SIN BlockCharacter(int sinId)
        {
            var sin = Get<SIN>(s => s.Id == sinId, s => s.Wallet, s => s.Character);
            sin.Blocked = true;
            return sin;
        }

        private void UnblockCharacter(SIN sin)
        {
            sin.Blocked = false;
        }

        private CorporationEnum GetCorporationForSku(Sku sku)
        {
            var corporation = sku.Corporation;
            if (corporation == null)
                corporation = Get<CorporationWallet>(w => w.Id == sku.CorporationId);
            foreach (CorporationEnum corEnum in Enum.GetValues(typeof(CorporationEnum)))
            {
                if (corEnum.ToString() == corporation.Alias)
                    return corEnum;
            }
            return CorporationEnum.unknown;
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
            var producttype = nomenklatura?.Specialisation?.ProductType;
            if (producttype == null)
            {
                var specialisation = Get<Specialisation>(s => s.Id == nomenklatura.SpecialisationId);
                if (specialisation == null)
                {
                    throw new Exception("Specialisation not found");
                }
                producttype = Get<ProductType>(p => p.Id == specialisation.ProductTypeId);
            }
            if (producttype == null)
                throw new Exception("ProductType not found");
            return BillingHelper.GetDiscountType(producttype.DiscountType);
        }

        private Price CreateNewPrice(Sku sku, ShopWallet shop, SIN sin)
        {
            decimal discount;
            try
            {
                var eService = new EreminService();
                discount = eService.GetDiscount(sin.Character.Model, GetDiscountTypeForSku(sku), GetCorporationForSku(sku));
            }
            catch
            {
                discount = 1;
            }
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

        private List<SIN> GetSinsInGame(params Expression<Func<SIN, object>>[] includes)
        {
            return GetList(s => (s.InGame ?? false) && s.Character.Game == CURRENTGAME, includes);
        }
        #endregion
    }
}
