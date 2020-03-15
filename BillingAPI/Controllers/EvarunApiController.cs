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
            catch(BillingAuthException e)
            {
                HandleAuthBillingException(e, guid);
                result.Message = e.Message;
                result.Status = false;
            }
            catch (BillingException ex)
            {
                HandleBillingException(ex, guid);
                result.Message = ex.Message;
                result.Status = false;

            }
            catch (Exception exc)
            {
                HandleException(exc, guid);
                result.Message = exc.ToString();
                result.Status = false;
            }
            return result;
        }

        protected Result RunAction(Action action, string logmessage)
        {
            var result = new Result();
            var guid = Guid.NewGuid();
            try
            {
                Start(logmessage, guid);
                action();
                result.Status = true;
                Finish(guid);
            }
            catch (BillingAuthException e)
            {
                HandleAuthBillingException(e, guid);
                result.Message = e.Message;
                result.Status = false;
            }
            catch (BillingException ex)
            {
                HandleBillingException(ex, guid);
                result.Message = ex.Message;
                result.Status = false;

            }
            catch (Exception exc)
            {
                HandleException(exc, guid);
                result.Message = exc.ToString();
                result.Status = false;
            }
            return result;
        }

        private void Start(string message, Guid guid)
        {
            Console.WriteLine($"Action {message} for {guid} started");
        }

        private void Finish(Guid guid)
        {
            Console.WriteLine($"Action for {guid} finished");
        }
        private void HandleAuthBillingException(BillingAuthException e, Guid guid)
        {
            Response.StatusCode = 401;
            Console.WriteLine($"AUTH ERROR for {guid}: {e.Message} ");
        }

        private void HandleBillingException(BillingException e, Guid guid)
        {
            Response.StatusCode = 422;
            Console.WriteLine($"CUSTOM ERROR for {guid}: {e.Message}");
        }
        private void HandleException(Exception e, Guid guid)
        {
            Response.StatusCode = 500;
            Console.WriteLine($"ERROR for {guid}: {e.ToString()}");
        }

    }
}