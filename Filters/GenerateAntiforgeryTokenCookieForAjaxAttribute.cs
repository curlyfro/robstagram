﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace robstagram.Filters
{
    public class GenerateAntiforgeryTokenCookieForAjaxAttribute : ActionFilterAttribute
    {
      public override void OnActionExecuted(ActionExecutedContext context)
      {
        var antiforgery = context.HttpContext.RequestServices.GetService<IAntiforgery>();

        // we can send the request token as a JavaScript-readable cookie, and angular will use it by default
        var tokens = antiforgery.GetAndStoreTokens(context.HttpContext);
        context.HttpContext.Response.Cookies.Append(
          "XSRF-TOKEN",
          tokens.RequestToken,
          new CookieOptions() { HttpOnly = false });
      }
    }
}
