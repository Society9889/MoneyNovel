using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoneyNovel.Controllers
{
    public class BaseController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (Session["FBID"] == null || (String) Session["FBID"] == "")
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "Account"));
            }
        }
    }
}
