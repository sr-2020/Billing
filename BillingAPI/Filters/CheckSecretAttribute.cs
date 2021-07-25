using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Settings;

namespace BillingAPI.Filters
{
    public class CheckSecretAttribute : ActionFilterAttribute
    {
        const string KEY = "secret";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dbSecret = IocContainer.Get<ISettingsManager>().GetValue(Core.Primitives.SystemSettingsEnum.secret); 
            if (filterContext.ActionArguments.ContainsKey(KEY))
            {
                if (filterContext.ActionArguments[KEY].ToString() == dbSecret)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Console.Error.WriteLine("WARNING!! HACK DETECTED!!");
            }
            throw new BillingException("WARNING Процесс пересчета уже запущен! Повторите позже");
            
        }
    }
}
