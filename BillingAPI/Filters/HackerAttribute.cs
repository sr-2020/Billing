using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Filters
{
    public class HackerAttribute : ActionFilterAttribute
    {
        string _secret = "8eaaf947d4b4";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Headers.ContainsKey("hacking"))
            {
                var hack = filterContext.HttpContext.Request.Headers["hacking"];
                if(hack == _secret)
                {
                    base.OnActionExecuting(filterContext);
                }
            }
            Console.Error.WriteLine("Неавторизованный взлом");
            filterContext.Result = new StatusCodeResult(403);
        }
    }
}
