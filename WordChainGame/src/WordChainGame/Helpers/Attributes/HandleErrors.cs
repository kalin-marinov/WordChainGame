using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WordChainGame.Data.Exceptions;

namespace WordChainGame.Helpers.Attributes
{

    public class HandleErrorsAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var ex = context.Exception;

            if (ex is TopicNotFoundException)
            {
                context.HttpContext.Response.StatusCode = 404;
            }
            else if(ex is WordNotFoundException)
            {
                context.HttpContext.Response.StatusCode = 404;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                context.HttpContext.Response.StatusCode = 401;
            }
            else if (context.Exception is ArgumentException)
            {
                context.HttpContext.Response.StatusCode = 400;
            }
            else
            {
                context.HttpContext.Response.StatusCode = 500;
            }

            context.Result = new JsonResult(ex.Message);
            base.OnException(context);
        }
    }
}
