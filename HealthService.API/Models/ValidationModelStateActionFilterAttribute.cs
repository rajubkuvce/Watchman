﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Collections.Generic;
using System.Linq;

namespace HealthService.API.Models
{
    public class ValidationModelStateActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                List<string> list = (from modelState in context.ModelState.Values from error in modelState.Errors select error.ErrorMessage).ToList();
                var res = context.ModelState.Values.SelectMany(modelstate => modelstate.Errors.Select(error => error.ErrorMessage)).ToList();
                context.Result = new BadRequestObjectResult(list);
            }

            base.OnActionExecuting(context);
        }
    }
}
