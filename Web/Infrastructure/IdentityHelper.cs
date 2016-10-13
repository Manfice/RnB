using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

namespace Web.Infrastructure
{
    public static class IdentityHelper
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            var manager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            var user =  manager.FindByIdAsync(id);
            return new MvcHtmlString(user.Result.Fio+" "+user.Result.Email);
        }
    }
}