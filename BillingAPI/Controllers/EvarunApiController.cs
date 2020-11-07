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
        protected DataResult<T> RunAction<T>(Func<T> action, string logmessage = "")
        {
            var guid = Guid.NewGuid();
            var result = new DataResult<T>();
            try
            {
                Start(logmessage, guid);
                result.Data = action();
                result.Status = true;
                Finish(guid);
            }
            catch (BillingAuthException e)
            {
                return HandleException(401, e.Message, guid, result);
            }
            catch (BillingException ex)
            {
                return HandleException(422, ex.Message, guid, result);
            }
            catch (ShopException se)
            {
                return HandleException(418, se.Message, guid, result);
            }
            catch(HttpResponseException re)
            {
                return HandleException((int)re.Response.StatusCode, re.Message, guid, result);
            }
            catch (Exception exc)
            {
                return HandleException(500, exc.ToString(), guid, result);
            }
            return result;
        }

        protected Result RunAction(Action action, string logmessage = "")
        {
            var guid = Guid.NewGuid();
            var result = new Result();
            try
            {
                Start(logmessage, guid);
                action();
                result.Status = true;
                Finish(guid);
            }
            catch (BillingAuthException e)
            {
                return HandleException(401, e.Message, guid, result);
            }
            catch (BillingException ex)
            {
                return HandleException(422, ex.Message, guid, result);
            }
            catch (ShopException se)
            {
                return HandleException(418, se.Message, guid, result);
            }
            catch (HttpResponseException re)
            {
                return HandleException((int)re.Response.StatusCode, re.Message, guid, result);
            }
            catch (Exception exc)
            {
                return HandleException(500, exc.ToString(), guid, result);
            }
            return result;
        }

        private void Start(string message, Guid guid)
        {
            if(string.IsNullOrEmpty(message))
            {
                return;
            }
            Console.WriteLine($"Action {message} for {guid} started");
        }

        private void Finish(Guid guid)
        {
            Console.WriteLine($"Action for {guid} finished");
        }
        private T HandleException<T>(int code, string message, Guid guid, T result) where T : Result 
        {
            Response.StatusCode = code;
            Console.Error.WriteLine($"ERROR for {guid}: {message}: {code}");
            result.Message = message;
            result.Status = false;
            return result;
        }
    }
}