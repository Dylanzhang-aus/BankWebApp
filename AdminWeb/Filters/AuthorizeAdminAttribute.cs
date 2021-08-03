using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace AdminWeb.Filters
{
    public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var adminID = context.HttpContext.Session.GetString("username");
            if (adminID == null)
                context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
