using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CoreApi.Middlewares
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger _log;

        public ExceptionFilter(ILoggerFactory log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            this._log = log.CreateLogger("Exception Filter");
        }


        public void OnException(ExceptionContext context)
        {
            // Check if exception on api
            if (context.HttpContext.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                // Check debug type
                // If header has key Debug
                // default is show json debug.
                // or html is show as html raw.
                if (context.HttpContext.Request.Headers.ContainsKey("debug") &&
                    context.HttpContext.Request.Headers["debug"].Equals("html"))
                {
                    return;
                }

                // Remark exception StackTrace
                var stackTrace = Regex.Replace(context.Exception.StackTrace, @"---([^/]+)$", "");

                var response = new DataResponse()
                {
                    StatusCode = 500,
                    Message = context.Exception.Message,
                    Debug = stackTrace.Trim()
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = 500,
                    DeclaredType = typeof(DataResponse)
                };

                this._log.LogError("ExceptionFilter", context.Exception);
            }
        }
    }
}
