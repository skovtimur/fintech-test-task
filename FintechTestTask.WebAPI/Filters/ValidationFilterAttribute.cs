using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FintechTestTask.WebAPI.Filters;

public class ValidationFilterAttribute : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid == false)
        {
            context.Result = new BadRequestObjectResult("ModelState is invalid");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}