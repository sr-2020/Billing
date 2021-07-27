using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    /// <summary>
    /// Get statistic on a game
    /// </summary>
    [Route("")]
    [ApiController]
    [AdminAuthorization]
    public class InfoController : EvarunApiController
    {
        [HttpPost("i-insolvents")]
        public void GetOnsolvents(bool beat, bool corporation, bool shop)
        {
            
        }

    }
}
