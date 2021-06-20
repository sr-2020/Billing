using Billing.Dto.Shop;
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
    public class HackerController : EvarunApiController
    {
     
        [HttpPost("h-transfer")]
        [Hacker]
        public DataResult<string> StealTransfer([FromBody] StealTransferRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("h-shop-transfer")]
        [Hacker]
        public DataResult<string> StealShopTransfer([FromBody] StealTransferRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("h-renta")]
        [Hacker]
        public DataResult<string> StealRenta([FromBody] StealRentaRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpGet("h-shop")]
        [Hacker]
        public DataResult<List<QRDto>> GetShopProducts(int shopId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("h-shop")]
        [Hacker]
        public DataResult<string> HackShop([FromBody] HackShopRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
