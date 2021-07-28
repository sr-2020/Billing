using Billing;
using BillingAPI.Model;
using IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Filters
{
    public class CorporationAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var manager = IocContainer.Get<IShopManager>();
            var character = 0;
            var corporation = 0;
            if (filterContext.ActionArguments.ContainsKey("character"))
            {
                int.TryParse(filterContext.ActionArguments["character"].ToString(), out character);
            }
            if (filterContext.ActionArguments.ContainsKey("corporation"))
            {
                int.TryParse(filterContext.ActionArguments["corporation"].ToString(), out corporation);
            }
            else
            {
                object model;
                if (filterContext.ActionArguments.TryGetValue("request", out model) && model is CorporationBasedRequest)
                {
                    corporation = ((CorporationBasedRequest)model).Corporation;
                }
            }
            if (!BillingHelper.IsAdmin(character) && !manager.HasAccessToCorporation(character, corporation))
            {
                filterContext.Result = new StatusCodeResult(403);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
