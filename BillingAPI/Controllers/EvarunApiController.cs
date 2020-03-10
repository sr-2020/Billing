using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using BillingAPI.Model;
using Core;
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
            catch (BillingException e)
            {
                result.Message = e.Message;
                result.Status = false;
                throw new HttpResponseException(System.Net.HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.Status = false;
                throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
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
            catch (BillingException e)
            {
                result.Message = e.Message;
                result.Status = false;
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.Status = false;
            }
            if (result.Status == false)
                throw new HttpResponseException(System.Net.HttpStatusCode.UnprocessableEntity);
            return result;
        }

    }
}