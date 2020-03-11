using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        protected DataResult<T> RunAction<T>(Func<T> action)
        {
            var result = new DataResult<T>();
            try
            {
                Console.WriteLine("Action started");
                result.Data = action();
                result.Status = true;
                Console.WriteLine("Action finished");
            }
            catch (BillingException e)
            {
                Console.WriteLine("---------WARNING---------");
                Console.WriteLine(e.Message);
                Console.WriteLine("---------WARNING---------");
                result.Message = e.Message;
                result.Status = false;
                Response.StatusCode = 422;
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------ERROR---------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("---------ERROR---------");
                result.Message = ex.ToString();
                result.Status = false;
                Response.StatusCode = 500;
            }
            return result;
        }

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
                Response.StatusCode = 422;
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                result.Message = ex.ToString();
                result.Status = false;
                Response.StatusCode = 500;
            }
            return result;
        }
    }
}