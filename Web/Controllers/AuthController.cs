using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Domen.Viewmodels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Web.Infrastructure;
using Web.Models;
using Web.Models.Viewmodels;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private AppUserManager UserMeneger => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private AppRoleManager RoleMeneger => HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.fromUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LogivVm model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserMeneger.FindAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Ошибка логина или пароля");
            }
            else
            {
                var ident = await UserMeneger.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthManager.SignOut();
                AuthManager.SignIn(new AuthenticationProperties {IsPersistent = false, ExpiresUtc = DateTimeOffset.MaxValue},ident);
                return RedirectToAction("Index","Home");
            }
            ViewBag.fromUrl = returnUrl;
            return View(model);
        }

        [Authorize]
        public ActionResult LogOut()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index","Home");
        }
    }
}