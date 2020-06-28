﻿using Billing;
using Core;
using IoC;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                character = int.Parse(filterContext.ActionArguments["character"].ToString());
            }
            if (filterContext.ActionArguments.ContainsKey("shop"))
            {
                shop = int.Parse(filterContext.ActionArguments["shop"].ToString());
            }
            if (!BillingHelper.IsAdmin(character) && !manager.HasAccessToShop(character, shop))
            {
                throw new BillingAuthException("character не имеет доступа к управлению магазином");
            }
        }
    }
}