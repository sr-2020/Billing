using Billing;
using BillingAPI.Model;
using Core;
using IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BillingAPI.Filters
{
    public class ShopAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
       {
            var manager = IocContainer.Get<IShopManager>();
            var character = 0;
            var shop = 0;
            if (filterContext.ActionArguments.ContainsKey("character"))
            {
                int.TryParse(filterContext.ActionArguments["character"].ToString(), out character);
            }
            if (filterContext.ActionArguments.ContainsKey("shop"))
            {
                int.TryParse(filterContext.ActionArguments["shop"].ToString(), out shop);
            }
            else
            {
                object model;
                if (filterContext.ActionArguments.TryGetValue("request", out model) && model is ShopBasedRequest)
                {
                    shop = ((ShopBasedRequest)model).Shop;
                }
            }
            if (!BillingHelper.IsAdmin(character) && !manager.HasAccessToShop(character, shop))
            {
                filterContext.Result = new StatusCodeResult(403);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
