using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    public abstract class EvarunApiController : ControllerBase
    {
        protected Result RunAction(Action action)
        {
            var result = new Result();
            try
            {
                action();
                result.Status = true;
            }
            catch (Exception e)
            {
                result.Message = e.ToString();
                result.Status = false;
            }
            return result;
        }

        protected DataResult<T> RunAction<T>(Func<T> action)
        {
            var result = new DataResult<T>();
            try
            {
                result.Data = action();
                result.Status = true;
            }
            catch (Exception e)
            {
                result.Message = e.ToString();
                result.Status = false;
            }
            return result;
        }

    }
}