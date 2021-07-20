using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Model;
using Core;
using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    /// <summary>
    /// async api entry point
    /// </summary>
    public class EvarunApiAsyncController : EvarunApiController
    {

        private async Task<DataResult<T>> HandleAsync<T>(Func<Task<T>> func, string actionName = "", string logmessage = "")
        {
            var guid = Guid.NewGuid();
            var result = new DataResult<T>();
            try
            {
                Start(actionName, logmessage, guid);
                result.Data = await func();
                result.Status = true;
                Finish(guid);
            }
            catch (BillingAuthException e)
            {
                return HandleException(403, e.Message, guid, result, actionName, logmessage);
            }
            catch (BillingNotFoundException e)
            {
                return HandleException(404, e.Message, guid, result, actionName, logmessage);
            }
            catch (BillingUnauthorizedException e)
            {
                return HandleException(401, e.Message, guid, result, actionName, logmessage);
            }
            catch (BillingException ex)
            {
                return HandleException(422, ex.Message, guid, result, actionName, logmessage);
            }
            catch (Exception exc)
            {
                return HandleException(500, exc.ToString(), guid, result, actionName, logmessage);
            }
            return result;
        }

        /// <summary>
        /// RunActionAsync
        /// </summary>
        protected async Task<DataResult<T>> RunActionAsync<T>(Func<Task<T>> func, string logmessage = "")
        {
            return await HandleAsync(func, func.Method.Name, logmessage);
        }
    }
}
