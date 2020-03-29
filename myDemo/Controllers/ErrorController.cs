using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace myDemo.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public ViewResult HandelExpetions(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMsg = "NotFound Resource";
                    ViewBag.OriginalPath = statusCodeResult.OriginalPath;
                    ViewBag.OriginalQueryString = statusCodeResult.OriginalQueryString;
                    break;
            }
            return View("NotFound");
        }



        [Route("Error")]
        public ViewResult GeneralHandelExpetions(int statusCode)
        {
            var ExpetionDetail = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ExceptionObj ex = new ExceptionObj
            {
                Message = ExpetionDetail.Error.Message.ToString(),
                Path = ExpetionDetail.Path,
                StackTrace = ExpetionDetail.Error.StackTrace

        };
    
            return View("Error", ex);
        }
    }

public class ExceptionObj
{
    public string Message { get; internal set; }
    public string Path { get; internal set; }
    public string StackTrace { get; internal set; }
}
}