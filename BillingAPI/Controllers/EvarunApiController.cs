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
                result.Status = ResultStatus.Success;
            }
            catch (Exception e)
            {
                result.Message = e.ToString();
                result.Status = ResultStatus.Error;
            }
            HttpContext.Response.StatusCode = (int)result.Status;
            return result;
        }

        protected DataResult<T> RunAction<T>(Func<T> action)
        {
            var result = new DataResult<T>();
            try
            {
                result.Data = action();
                result.Status = ResultStatus.Success;
            }
            catch (Exception e)
            {
                result.Message = e.ToString();
                result.Status = ResultStatus.Error;
            }
            HttpContext.Response.StatusCode = (int)result.Status;
            return result;
        }

    }
}