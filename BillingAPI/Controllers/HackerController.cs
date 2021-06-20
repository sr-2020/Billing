using Billing.Dto.Shop;
using Billing.DTO;
using BillingAPI.Filters;
using BillingAPI.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Controllers
{
    [Route("")]
    [Hacker]
    public class HackerController : EvarunApiController
    {
        [HttpPost("h-transfer")]
        public DataResult<string> StealTransfer([FromBody] StealTransferRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("h-shop-transfer")]
        public DataResult<string> StealShopTransfer([FromBody] StealTransferRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("h-renta")]
        public DataResult<string> StealRenta([FromBody] StealRentaRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpGet("h-shop")]
        public DataResult<ShopDetailedDto> GetShop(int shopId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("h-shops")]
        public DataResult<List<ShopDto>> GetShops()
        {
            throw new NotImplementedException();
        }

        [HttpPost("h-shop")]
        public DataResult<string> HackShop([FromBody] HackShopRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
