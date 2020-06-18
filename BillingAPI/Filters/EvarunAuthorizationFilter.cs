using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Filters
{
    public class EvarunAuthorizationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Headers.ContainsKey("X-User-Id"))
            {
                context.ActionArguments.Add("character", context.HttpContext.Request.Headers["X-User-Id"]);
            }
            else
            {
                //TODO redirect
                context.ActionArguments.Add("character", 0);
            }
            
        }
    }
}
