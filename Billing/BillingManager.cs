using Billing.Dto;
using Billing.Dto.Shop;
using Billing.DTO;
using Core;
using Core.Exceptions;
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
    public interface IBillingManager : IAdminManager
    {
        #region application

        Transfer CreateTransferMIRSIN(int characterTo, decimal amount);

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
        RentaDto ConfirmRenta(int modelId, int priceId, int count = 1);
        List<SkuDto> GetSkus(int corporationId, int nomenklaturaId, bool? enabled, int id = -1);
        ProductType GetExtProductType(string name);
        Nomenklatura GetExtNomenklatura(string name);
        Sku GetExtSku(string name);
        List<Contract> GetContrats(int shopid, int corporationId);
        #endregion

        #region jobs

        JobLifeStyleDto ProcessCharacterBeat(int sinId, decimal karmaCount, bool dividents1, bool dividents2, bool dividents3, JobLifeStyleDto dto);

        #endregion

        #region admin

        List<SIN> GetSinsInGame();
        List<CharacterDto> GetCharactersInGame();
        List<TransferDto> GetTransfersByRenta(int rentaID);

        #endregion
    }

    public class BillingManager : AdminManager, IBillingManager
    {



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
                CorporationId = corporation,
                Status = (int)ContractStatusEnum.Approved
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
            var result = new TransferSum();
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Character);
            if (sin == null)
                throw new BillingException("sin not found");
            var sum = new RentaSumDto();
            var list = GetListAsNoTracking<Renta>(r => r.Sin.Character.Model == modelId,
                r => r.Sku.Nomenklatura.Specialisation.ProductType,
                r => r.Sku.Corporation,
                r => r.Shop,
                r => r.Sin.Passport,
                r => r.Sin.Character)
                .Select(r =>
                    new RentaDto
                    {
                        ModelId = modelId.ToString(),
                        CharacterName = r.Sin.Passport?.PersonName ?? "Unknown",
                        FinalPrice = Math.Round(BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring), 2),
                        ProductType = r.Sku.Nomenklatura.Specialisation.ProductType.Name,
                        Shop = r.Shop.Name,
                        NomenklaturaName = r.Sku.Nomenklatura.Name,
                        SkuName = r.Sku.Name,
                        Corporation = r.Sku.Corporation.Name,
                        RentaId = r.Id,
                        HasQRWrite = r.HasQRWrite,
                        QRRecorded = r.QRRecorded,
                        DateCreated = r.DateCreated,
                        Specialisation = r.Sku.Nomenklatura.Specialisation.Name
                    }).ToList();
            sum.Rentas = list;
            sum.Sum = list.Sum(r => r.FinalPrice);
            return sum;

        }

        public RentaDto ConfirmRenta(int modelId, int priceId, int count = 1)
        {
            var sin = BillingBlocked(modelId, s => s.Wallet, s => s.Character, s => s.Passport);
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
            if (dateTill < DateTime.Now)
                throw new BillingException($"Персональное предложение больше не действительно, оно истекло {dateTill:HH:mm:ss}");
            var allowed = SkuAllowed(price.ShopId, price.SkuId);
            if (allowed == null)
                throw new BillingException("Sku недоступно для продажи в данный момент");
            price.BasePrice *= count;
            var finalPrice = BillingHelper.GetFinalPrice(price.BasePrice, price.Discount, price.CurrentScoring);
            if (sin.Wallet.Balance - price.FinalPrice < 0)
            {
                throw new BillingException("Недостаточно средств");
            }
            price.Sku.Count -= count;
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
                LifeStyle = price.Sku.Nomenklatura.Lifestyle,
                Count = count
            };
            Add(renta);
            price.Confirmed = true;
            SaveContext();
            ProcessBuyScoring(sin, price.Sku);
            var mir = GetMIR();
            ProcessRenta(renta, mir, sin, true);
            SaveContext();
            if (instantConsume)
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
                FinalPrice = finalPrice
            };
            return dto;
        }

        public JobLifeStyleDto ProcessCharacterBeat(int sinId, decimal karmaCount, bool dividents1, bool dividents2, bool dividents3, JobLifeStyleDto dto)
        {
            var sin = BlockCharacter(sinId);
            SaveContext();
            var mir = GetMIR();
            decimal income = 0;
            decimal outcome = 0;
            //ability
            if (dividents1)
            {
                var dk1 = _settings.GetDecimalValue(SystemSettingsEnum.dividents1_k);
                AddNewTransfer(mir, sin.Wallet, dk1, "Дивиденды *");
                income += dk1;
            }
            if (dividents2)
            {
                var dk2 = _settings.GetDecimalValue(SystemSettingsEnum.dividents2_k);
                AddNewTransfer(mir, sin.Wallet, dk2, "Дивиденды **");
                income += dk2;
            }
            if (dividents3)
            {
                var dk3 = _settings.GetDecimalValue(SystemSettingsEnum.dividents3_k);
                AddNewTransfer(mir, sin.Wallet, dk3, "Дивиденды ***");
                income += dk3;
            }
            //karma
            if (karmaCount > 0)
            {
                var k = _settings.GetDecimalValue(SystemSettingsEnum.karma_k);
                var karmasum = k * karmaCount;
                income += karmasum;
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
            outcome -= rentas.Sum(r => BillingHelper.GetFinalPrice(r.BasePrice, r.Discount, r.CurrentScoring));
            sin.Wallet.IncomeOutcome = income - outcome;
            dto = CalculateLifeStyle(sin.Wallet, dto, income, outcome);
            UnblockCharacter(sin);
            SaveContext();
            return dto;
        }

        private JobLifeStyleDto CalculateLifeStyle(Wallet wallet, JobLifeStyleDto dto, decimal income, decimal outcome)
        {
            if (wallet.IsIrridium)
            {
                dto.Irridium++;
                return dto;
            }
            if (wallet.Balance < 0)
            {
                dto.Insolvent++;
                return dto;
            }
            var forecast = BillingHelper.GetForecast(wallet);
            if (dto.Min == null || ((dto.Min ?? 0) > wallet.Balance))
                dto.Min = wallet.Balance;
            if (dto.Max == null || ((dto.Max ?? 0) < wallet.Balance))
                dto.Max = wallet.Balance;
            if (dto.ForecastMin == null || ((dto.ForecastMin ?? 0) > forecast))
                dto.ForecastMin = forecast;
            if (dto.ForecastMax == null || ((dto.ForecastMax ?? 0) < forecast))
                dto.ForecastMax = forecast;
            dto.SumAll += wallet.Balance;
            dto.SumKarma += income;
            dto.SumRents += outcome;
            dto.ForecastSumAll += forecast;
            return dto;
        }

        /// <summary>
        /// НЕ ВЫПОЛНЯЕТСЯ SAVECONTEXT
        /// </summary>
        private void ProcessRenta(Renta renta, Wallet mir, SIN sin, bool first = false)
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
                CloseOverdraft(renta, mir, sin, first);
                //close overdraft here
                var allOverdrafts = GetList<Transfer>(t => t.Overdraft && t.WalletFromId == sin.Wallet.Id && t.RentaId > 0);
                foreach (var overdraft in allOverdrafts)
                {
                    overdraft.Overdraft = false;
                    var closingRenta = Get<Renta>(r => r.Id == overdraft.RentaId, r => r.Sku.Corporation, r => r.Shop.Wallet);
                    CloseOverdraft(closingRenta, mir, sin);
                }
            }
            else
            {
                AddNewTransfer(sin.Wallet, mir, finalPrice, $"Рентный платеж: {renta.Sku.Name} в {renta.Shop.Name}", false, renta.Id, true);
            }
        }

        private void CloseOverdraft(Renta renta, Wallet mir, SIN sin, bool first = false)
        {
            var comission = BillingHelper.CalculateComission(renta.BasePrice, renta.ShopComission);
            //create KPI here
            renta.Sku.Corporation.CurrentKPI += renta.BasePrice;
            if (first)
                renta.Sku.Corporation.SkuSold += renta.BasePrice;
            //comission
            AddNewTransfer(mir, renta.Shop.Wallet, comission, $"Рентное начисление: {renta.Sku.Name} в {renta.Shop.Name} от {sin.Passport.PersonName} ({sin.Passport.Sin})", false, renta.Id, false);
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
                case "EdibleFood":
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
                case "Charity":
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
            var sin = BillingBlocked(modelId, s => s.Scoring, s => s.Character);
            var sku = SkuAllowed(shopid, skuid);
            if (sku == null)
                throw new BillingException("sku недоступен для продажи");
            var shop = GetAsNoTracking<ShopWallet>(s => s.Id == shopid);
            if (shop == null || sin == null)
                throw new Exception("some went wrong");
            var price = CreateNewPrice(sku, shop, sin);
            var dto = new PriceShopDto(new PriceDto(price, true), sku.Corporation);
            return dto;
        }

        public BalanceDtoOld GetBalanceOld(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Scoring, s => s.Passport.Metatype);
            var lifestyle = BillingHelper.GetLifeStyleDto();
            var balance = new BalanceDtoOld
            {
                ModelId = modelId,
                CurrentBalance = BillingHelper.Round(sin.Wallet.Balance),
                CurrentScoring = sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative,
                SIN = sin.Passport.Sin,
                ForecastLifeStyle = lifestyle.GetForecastLifeStyle(sin.Wallet).ToString(),
                LifeStyle = lifestyle.GetLifeStyle(sin.Wallet).ToString(),
                PersonName = sin.Passport.PersonName,
                Metatype = sin.Passport.Metatype?.Name ?? "неизвестно",
                Citizenship = sin.Passport.Citizenship ?? "неизвестно",
                Nationality = "устарело на Амуре",
                Status = "устарело на Амуре",
                Nation = "устарело на Амуре",
                Viza = sin.Passport.Viza ?? "неизвестно",
                Pledgee = sin.Passport.Mortgagee ?? "неизвестно"
            };
            return balance;
        }

        public BalanceDto GetBalance(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Scoring, s => s.Passport.Metatype);
            var inss = ProductTypeEnum.Insurance.ToString();
            var insur = GetList<Renta>(r => r.Sku.Nomenklatura.Specialisation.ProductType.Alias == inss && r.SinId == sin.Id, r => r.Sku)
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();
            var lics = ProductTypeEnum.Licences.ToString();
            var licences = GetList<Renta>(r => r.Sku.Nomenklatura.Specialisation.ProductType.Alias == lics && r.SinId == sin.Id, r => r.Sku.Nomenklatura)
                .OrderByDescending(r => r.DateCreated)
                .GroupBy(l => l.Sku.NomenklaturaId)
                .Select(g => g.FirstOrDefault()?.Sku?.Name)
                .ToList();
            var lifestyle = BillingHelper.GetLifeStyleDto();
            var balance = new BalanceDto
            {
                ModelId = modelId,
                CurrentBalance = BillingHelper.Round(sin.Wallet.Balance),
                CurrentScoring = Math.Round(sin.Scoring.CurrentFix + sin.Scoring.CurerentRelative, 2),
                SIN = sin.Passport.Sin,
                LifeStyle = lifestyle.GetLifeStyle(sin.Wallet).ToString(),
                ForecastLifeStyle = lifestyle.GetForecastLifeStyle(sin.Wallet).ToString(),
                PersonName = sin.Passport.PersonName,
                Metatype = sin.Passport.Metatype?.Name ?? "неизвестно",
                Citizenship = sin.Passport.Citizenship ?? "неизвестно",
                Nationality = "устарело на Амуре",
                Status = "устарело на Амуре",
                Nation = "устарело на Амуре",
                Viza = sin.Passport.Viza ?? "неизвестно",
                Pledgee = sin.Passport.Mortgagee ?? "неизвестно",
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
            var sin = GetSINByModelId(modelId, s => s.Passport);
            if (sin == null)
                throw new Exception("sin not found");
            return sin.Passport.Sin;
        }

        public int GetModelIdBySinString(string sinString)
        {
            var sin = GetSINBySinText(sinString, s => s.Character);
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

        public Transfer CreateTransferMIRSIN(int characterTo, decimal amount)
        {
            var to = BillingBlocked(characterTo, s => s.Wallet);
            var from = GetMIR();
            var comment = "Перевод от международного банка";
            var transfer = AddNewTransfer(from, to.Wallet, amount, comment);
            SaveContext();
            return transfer;
        }

        public List<SIN> GetSinsInGame()
        {
            return GetSinsInGame(s => s.Character, s => s.Wallet, s => s.Scoring, s => s.Passport);
        }

        public List<CharacterDto> GetCharactersInGame()
        {
            var result = GetSinsInGame(s => s.Character, s => s.Passport).Select(s => new CharacterDto { PersonName = s.Passport?.PersonName, ModelId = s.Character.Model.ToString() }).ToList();
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
            var sin = Get<SIN>(s => s.Id == sinId, s => s.Wallet, s => s.Character, s => s.Passport);
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
