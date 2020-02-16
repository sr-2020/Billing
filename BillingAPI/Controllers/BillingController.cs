using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using BillingAPI.Model;
using IoC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : EvarunApiController
    {
        private readonly Lazy<IBillingManager> _manager = new Lazy<IBillingManager>(IocContainer.Get<IBillingManager>);

        #region transfer
        [HttpGet("transfer/maketransfersinsin")]
        public Result MakeTransferSINSIN(string sin1, string sin2)
        {
            throw new NotImplementedException();
        }
        [HttpGet("transfer/maketransfersinleg")]
        public Result MakeTransferSINLeg(string sin, string leg)
        {
            throw new NotImplementedException();
        }
        [HttpGet("transfer/maketransferlegsin")]
        public Result MakeTransferLegSIN(string leg, string sin)
        {
            throw new NotImplementedException();
        }
        [HttpGet("transfer/maketransferlegleg")]
        public Result MakeTransferLegLeg(string leg1, string leg2)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region renta
        [HttpGet("renta/createprice")]
        public Result CreatePrice()
        {
            throw new NotImplementedException();
        }
        [HttpGet("renta/createpricebyparams ")]
        public Result CreatePriceByParams()
        {
            throw new NotImplementedException();
        }
        [HttpGet("renta/confirmrenta ")]
        public Result ConfirmRenta()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region info
        [HttpGet("info/base")]
        public Result Base(string id)
        {
            throw new NotImplementedException();
        }
        [HttpGet("info/advanced")]
        public Result Advanced(string id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}