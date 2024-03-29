﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Model;
using Core;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    /// <summary>
    /// api entry point 
    /// </summary>
    public abstract class EvarunApiController : ControllerBase
    {
        private DataResult<T> Handle<T>(Func<T> func, Action action, string actionName = "", string logmessage = "")
        {
            var guid = Guid.NewGuid();
            var result = new DataResult<T>();
            try
            {
                Start(actionName, logmessage, guid);
                if(func == null)
                {
                    action();
                }
                else
                {
                    result.Data = func();
                }
                result.Status = true;
                Finish(guid);
            }
            catch (BillingAuthException e)
            {
                return HandleWarning(403, e.Message, guid, result, actionName, logmessage);
            }
            catch (BillingNotFoundException e)
            {
                return HandleWarning(404, e.Message, guid, result, actionName, logmessage);
            }
            catch (BillingUnauthorizedException e)
            {
                return HandleWarning(401, e.Message, guid, result, actionName, logmessage);
            }
            catch (BillingException ex)
            {
                return HandleWarning(422, ex.Message, guid, result, actionName, logmessage);
            }
            catch (Exception exc)
            {
                return HandleException(500, exc.ToString(), guid, result, actionName, logmessage);
            }
            return result;
        }

        protected DataResult<T> RunAction<T>(Func<T> func, string logmessage = "")
        {
            return Handle(func, null, func.Method.Name, logmessage);
        }

        protected Result RunAction(Action action, string logmessage = "")
        {
            return Handle<string>(null, action, action.Method.Name, logmessage);
        }

        protected void Start(string action, string message, Guid guid)
        {
            if(string.IsNullOrEmpty(message))
            {
                return;
            }
            Console.WriteLine($"Action {action}: {message} for {guid} started");
        }

        protected void Finish(Guid guid)
        {
            //Console.WriteLine($"Action for {guid} finished");
        }

        protected T HandleException<T>(int code, string message, Guid guid, T result, string action, string logmessage) where T : Result 
        {
            Response.StatusCode = code;
            Console.Error.WriteLine($"ERROR for {logmessage}:{guid}({action}): {message}: code {code}");
            result.Message = message;
            result.Status = false;
            return result;
        }
        protected T HandleWarning<T>(int code, string message, Guid guid, T result, string action, string logmessage) where T : Result
        {
            Response.StatusCode = code;
            Console.WriteLine($"ERROR for {logmessage}:{guid}({action}): {message}: code {code}");
            result.Message = message;
            result.Status = false;
            return result;
        }

    }
}