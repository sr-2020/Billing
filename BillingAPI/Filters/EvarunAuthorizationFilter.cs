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
        private void AddCharacter(ActionExecutingContext context, string value)
        {
            int character;
            if (int.TryParse(value, out character))
            {
                if(context.ActionArguments.ContainsKey("character"))
                {
                    context.ActionArguments["character"] = character;
                }
                else
                {
                    context.ActionArguments.Add("character", character);
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.HttpContext.Request.Headers.ContainsKey("X-User-Id"))
            {
                AddCharacter(context, context.HttpContext.Request.Headers["X-User-Id"]);
            }
            else
            {
                AddCharacter(context, "0");
                //TODO redirect
            }
            
        }
    }
}
