using DlnaController.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DlnaController.Web.Filters
{
    public sealed class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public CustomExceptionFilterAttribute(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("Exception");
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is FriendlyException friendlyException)
            {
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult(new ApiResult(friendlyException.Code, friendlyException.Message));
            }
            else
            {
                var innerException = context.Exception.InnerException;
                var deep = 3;
                string message = context.Exception.Message;
                while (innerException != null && deep > 0)
                {
                    message += innerException.Message;
                    innerException = innerException.InnerException;
                    deep--;
                }
                _logger.LogError(context.Exception, $"{context.ActionDescriptor.DisplayName}: {message}, {context.Exception.StackTrace}");
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult(new ApiResult(-500, message));
            }
            context.ExceptionHandled = true;
        }
    }

}