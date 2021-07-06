using Billing.Dto;
using Billing.Dto.Shop;
using Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Services
{
    public class AdminService : BaseService
    {
        public FullUserDto GetFullUser(int modelid)
        {
            var user = new FullUserDto(modelid)
            {
                Sin = Factory.Billing.GetBalance(modelid),
                Transfers = Factory.Billing.GetTransfers(modelid),
                Rents = Factory.Billing.GetRentas(modelid),
                Scoring = Factory.Scoring.GetFullScoring(modelid),
                IsAdmin = BillingHelper.IsAdmin(modelid)
            };
            return user;
        }

        public SessionDto GetSessionInfo(int character)
        {
            var result = new SessionDto();
            try
            {
                var ls = BillingHelper.GetLifeStyleDto();
                result.LifeStyle = new SessionDto.LifeStyleDto(ls);
                var cycle = Factory.Job.GetLastCycle();
                result.Cycle = new SessionDto.CycleDto(cycle);
                var beatCharacters = Factory.Job.GetLastBeatAsNoTracking(Core.Primitives.BeatTypes.Characters);
                var beatItems = Factory.Job.GetLastBeatAsNoTracking(Core.Primitives.BeatTypes.Items);
                var jsoncharacters = Factory.Settings.GetValue(Core.Primitives.SystemSettingsEnum.beat_characters_dto);
                var lsDto = Serialization.Serializer.Deserialize<JobLifeStyleDto>(jsoncharacters);
                result.BeatCharacters = new SessionDto.BeatCharactersDto(beatCharacters, lsDto);
                var corps = Factory.Shop.GetCorporationDtos(c => true);
                var shops = Factory.Shop.GetShops(character, s => true);
                result.BeatItems = new SessionDto.BeatItemsDto(beatItems, corps, shops);
                result.Deploy = Environment.GetEnvironmentVariable(SystemHelper.Billing);
                var sin = Factory.Billing.GetSINByModelId(character, s => s.Passport);
                result.PersonName = BillingHelper.GetPassportName(sin.Passport);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
            return result;
        }
    }
}
