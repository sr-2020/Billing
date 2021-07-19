using Billing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Filters
{
    public class AdminAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var character = 0;
            if (filterContext.ActionArguments.ContainsKey("character"))
            {
                int.TryParse(filterContext.ActionArguments["character"].ToString(), out character);
            }
            if (!BillingHelper.IsAdmin(character))
            {
                filterContext.Result = new StatusCodeResult(403);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
