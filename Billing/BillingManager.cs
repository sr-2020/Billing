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
        BalanceDto GetBalance(int modelId);
        RentaSumDto GetRentas(int modelId);
        #endregion

        #region web
        PriceShopDto GetPriceByQR(int character, string qrid);
        PriceShopDto GetPrice(int modelId, int shop, int sku);
        void BreakContract(int corporation, int shop);
        Contract CreateContract(int corporation, int shop);
        RentaDto ConfirmRenta(int modelId, int priceId, int count = 1);
        List<SkuDto> GetSkuDtos(int corporationId, int nomenklaturaId, bool? enabled, int id = -1);
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
        #endregion

        #region events
        void DropInsurance(int modelId);
        SIN DropCharacter(int modelId);
        SIN RestoreCharacter(int modelId);
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

        public List<SkuDto> GetSkuDtos(int corporationId, int nomenklaturaId, bool? enabled, int id = -1)
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
                    new RentaDto(r))
                    .ToList();
            sum.Rentas = list;
            sum.Sum = list.Sum(r => r.FinalPrice);
            return sum;

        }

        public RentaDto ConfirmRenta(int modelId, int priceId, int count = 1)
        {
            var renta = CreateRenta(modelId, priceId, count);
            EreminPushAdapter.SendNotification(modelId, "Покупка совершена", $"Вы купили {renta.Sku.Name}");
            var dto = new RentaDto(renta);
            return dto;
        }

        public JobLifeStyleDto ProcessCharacterBeat(int sinId, decimal karmaCount, bool dividends1, bool dividends2, bool dividends3, JobLifeStyleDto dto)
        {
            var sin = BlockCharacter(sinId, s => s.Wallet, s => s.Character, s => s.Passport, s => s.Scoring);
            SaveContext();
            var mir = GetMIR();
            decimal income = 0;
            decimal outcome = 0;
            //ability
            if (dividends1)
            {
                var dk1 = _settings.GetDecimalValue(SystemSettingsEnum.dividents1_k);
                AddNewTransfer(mir, sin.Wallet, dk1, "Дивиденды *");
                income += dk1;
                dto.SumDividends += dk1;
            }
            if (dividends2)
            {
                var dk2 = _settings.GetDecimalValue(SystemSettingsEnum.dividents2_k);
                AddNewTransfer(mir, sin.Wallet, dk2, "Дивиденды **");
                income += dk2;
                dto.SumDividends += dk2;
            }
            if (dividends3)
            {
                var dk3 = _settings.GetDecimalValue(SystemSettingsEnum.dividents3_k);
                AddNewTransfer(mir, sin.Wallet, dk3, "Дивиденды ***");
                income += dk3;
                dto.SumDividends += dk3;
            }
            dto.SumDividends += income;
            //karma
            if (karmaCount > 0)
            {
                var k = _settings.GetDecimalValue(SystemSettingsEnum.karma_k);
                var karmasum = k * karmaCount;
                income += karmasum;
                dto.SumKarma += karmasum;
                AddNewTransfer(mir, sin.Wallet, karmasum, "пассивный доход");
            }
            //rentas
            var rentas = GetList<Renta>(r => r.SinId == sin.Id, r => r.Shop.Wallet, r => r.Sku.Corporation.Wallet);
            foreach (var renta in rentas)
            {
                ProcessRenta(renta, mir, sin);
            }
            //metatype
            if (sin.Passport.MetatypeId != sin.OldMetaTypeId)
            {
                var scoring = IoC.IocContainer.Get<IScoringManager>();
                scoring.OnMetatypeChanged(sin);
                sin.OldMetaTypeId = sin.Passport.MetatypeId;
            }
            //insurance
            var insurance = GetInsurance(sin.CharacterId);
            if (insurance?.LifeStyle != sin.OldInsurance)
            {
                if ((insurance?.LifeStyle ?? 0) > 0 != (sin.OldInsurance ?? 0) > 0)
                {
                    var scoring = IoC.IocContainer.Get<IScoringManager>();
                    scoring.OnInsuranceChanged(sin, (insurance?.LifeStyle ?? 0) > 0);
                }
                sin.OldInsurance = insurance?.LifeStyle;
            }
            //summary
            AddScoring(sin.Scoring, dto);
            //forecast
            outcome -= rentas.Sum(r => BillingHelper.GetFinalPrice(r));

            sin.Wallet.IncomeOutcome = income - outcome;
            dto = AddLifeStyle(sin.Wallet, dto);
            UnblockCharacter(sin);
            SaveContext();
            return dto;
        }

        private JobLifeStyleDto AddScoring(Scoring scoring, JobLifeStyleDto dto)
        {
            var scoringvalue = BillingHelper.GetFullScoring(scoring);
            dto.ScoringComposition *= scoringvalue;
            if (dto.ScoringMin == 0 || scoringvalue < dto.ScoringMin)
            {
                dto.ScoringMin = scoringvalue;
            }
            if (dto.ScoringMax == 0 || scoringvalue > dto.ScoringMax)
            {
                dto.ScoringMax = scoringvalue;
            }
            return dto;
        }

        private JobLifeStyleDto AddLifeStyle(Wallet wallet, JobLifeStyleDto dto)
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
            dto.ForecastSumAll += forecast;
            return dto;
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
            var sin = BillingBlocked(modelId, s => s.Scoring, s => s.Passport);
            var sku = SkuAllowed(shopid, skuid, s => s.Corporation, s => s.Nomenklatura.Specialisation.ProductType);
            var shop = GetAsNoTracking<ShopWallet>(s => s.Id == shopid);
            if (shop == null || sin == null)
                throw new Exception("some went wrong");
            var price = CreateNewPrice(sku, shop, sin);
            var dto = new PriceShopDto(new PriceDto(price, true), sku.Corporation);
            return dto;
        }

        public BalanceDto GetBalance(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Scoring, s => s.Passport.Metatype);
            var insur = GetInsurance(modelId, r => r.Sku);
            var lics = ProductTypeEnum.Licences.ToString();
            var licences = GetList<Renta>(r => r.Sku.Nomenklatura.Specialisation.ProductType.Alias == lics && r.SinId == sin.Id, r => r.Sku.Nomenklatura)
                .OrderByDescending(r => r.DateCreated)
                .GroupBy(l => l.Sku.NomenklaturaId)
                .Select(g => g.FirstOrDefault()?.Sku?.Name)
                .ToList();
            var lifestyle = BillingHelper.GetLifeStyleDto();
            var balance = new BalanceDto(sin)
            {
                ModelId = modelId,
                LifeStyle = lifestyle.GetLifeStyle(sin.Wallet).ToString(),
                ForecastLifeStyle = lifestyle.GetForecastLifeStyle(sin.Wallet).ToString(),
                Insurance = insur?.Sku?.Name ?? "нет страховки",
                Licenses = licences
            };
            return balance;
        }

        public TransferSum GetTransfers(int modelId)
        {
            var result = new TransferSum();
            var sin = GetSINByModelId(modelId, s => s.Wallet, s => s.Passport);
            if (sin == null)
                throw new BillingException("sin not found");
            var listFrom = GetListAsNoTracking<Transfer>(t => t.WalletFromId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            var listTo = GetListAsNoTracking<Transfer>(t => t.WalletToId == sin.WalletId, t => t.WalletFrom, t => t.WalletTo);
            var owner = BillingHelper.GetPassportName(sin.Passport);
            result.Transfers = CreateTransfersDto(listFrom, listTo, owner);
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

        public FullUserDto GetFullUser(int modelid)
        {
            return new FullUserDto();
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

        public void DropInsurance(int modelId)
        {
            var insurance = GetInsurance(modelId);
            if (insurance != null)
            {
                insurance.Expired = true;
                SaveContext();
            }
        }

        public SIN DropCharacter(int modelId)
        {
            var sin = GetSINByModelId(modelId, s => s.Wallet);
            sin.InGame = false;
            var rents = GetList<Renta>(r => r.SinId == sin.Id);
            SaveContext();
            foreach (var renta in rents)
            {
                if (!string.IsNullOrEmpty(renta.QRRecorded))
                {
                    try
                    {
                        CleanRenta(renta);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.ToString());
                    }
                }
            }
            return sin;
        }
        public SIN RestoreCharacter(int modelId)
        {
            var sin = GetDisabledByModelId(modelId);
            sin.InGame = true;
            SaveContext();
            return sin;
        }

        #region private

        private SIN BlockCharacter(int sinId, params Expression<Func<SIN, object>>[] includes)
        {
            var sin = Get(s => s.Id == sinId, includes);
            sin.Blocked = true;
            return sin;
        }

        private void UnblockCharacter(SIN sin)
        {
            sin.Blocked = false;
        }

        private List<SIN> GetSinsInGame(params Expression<Func<SIN, object>>[] includes)
        {
            return GetList(s => (s.InGame ?? false) && s.Character.Game == CURRENTGAME, includes);
        }

        protected Renta GetInsurance(int modelId, params Expression<Func<Renta, object>>[] includes)
        {
            var inss = ProductTypeEnum.Insurance.ToString();
            var insurance = GetList(r => r.Sku.Nomenklatura.Specialisation.ProductType.Alias == inss && r.Sin.Character.Model == modelId && !r.Expired, includes)
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();
            return insurance;
        }
        #endregion
    }
}
