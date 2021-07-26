using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Model;
using Core;
using Core.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;
using Settings;

namespace BillingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemSettingsController : EvarunApiController
    {
        private readonly Lazy<ISettingsManager> _manager = new Lazy<ISettingsManager>(IocContainer.Get<ISettingsManager>);
        
        /// <summary>
        /// Get list of all system settings
        /// </summary>
        /// <returns></returns>
        [HttpGet("ShowAll")]
        public DataResult<List<SystemSettings>> ShowAll()
        {
            var manager = IocContainer.Get<ISettingsManager>();
            var result = RunAction(() => manager.GetAllSettings(), "settings/ShowAll");
            return result;
        }
        [HttpGet("GetValue")]
        public DataResult<string> GetValue(string key)
        {
            var manager = IocContainer.Get<ISettingsManager>();
            var result = RunAction(() => manager.GetValue(key), "settings/GetValue");
            return result;
        }
        [HttpPost("SetValue")]
        public DataResult<SystemSettings> SetValue(string key, string value)
        {
            var manager = IocContainer.Get<ISettingsManager>();
            var result = RunAction(() => manager.SetValue(key, value), "settings/SetValue");
            return result;
        }

    }
}