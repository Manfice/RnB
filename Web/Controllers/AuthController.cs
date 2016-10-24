using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Domen.Viewmodels;
using Web.Helpers;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private AppUserManager UserMeneger => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private AppRoleManager RoleMeneger => HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;
        private readonly IAuth _auth;

        public AuthController(IAuth auth)
        {
            _auth = auth;
        }

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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(CustomerViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index", "Home");
            var user = await UserMeneger.FindByEmailAsync(model.Email.ToLower());
            if (user==null)
            {
                var pass = System.Web.Security.Membership.GeneratePassword(8, 3);
                user = new AppUser { Email = model.Email, UserName = model.Email, PhoneNumber = model.Phone };
                await UserMeneger.CreateAsync(user, pass);
                var result = await UserMeneger.AddToRoleAsync(user.Id, "Customer");
                if (result.Succeeded)
                {
                    var a = Url.Action("Login", "Auth", new { pass = pass, title = Guid.NewGuid() });
                    var link = $"<a href=\"{a}\" target=\"_blanck\">Завершить регистрацию</a>";

                }
            }
            AddErrorsFormResult(new IdentityResult(new []{"Пользователь с таким E-mail уже зарегистрирован."}));
            return RedirectToAction("Index","Home");
        }
        
        [Authorize]
        public ActionResult LogOut()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index","Home");
        }

        private void AddErrorsFormResult(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item);
            }
        }
    }
}