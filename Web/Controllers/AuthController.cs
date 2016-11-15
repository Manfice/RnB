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
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
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
                AuthManager.SignIn(new AuthenticationProperties {IsPersistent = true},ident);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            ViewBag.fromUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Register(CustomerViewModel model, string returnUrl="")
        {
            if (!ModelState.IsValid) return View(model);
            
            var user = await UserMeneger.FindByEmailAsync(model.Email.ToLower());
            if (user == null)
            {
                var pass = System.Web.Security.Membership.GeneratePassword(8, 3);
                user = new AppUser {Email = model.Email, UserName = model.Email, PhoneNumber = model.Phone};
                var u = await UserMeneger.CreateAsync(user, pass);
                var result = await UserMeneger.AddToRoleAsync(user.Id, "Customer");
                model.UserId = user.Id;
                await _auth.RegCustomer(model);
                if (result.Succeeded)
                {
                    var body = System.IO.File.ReadAllText(Server.MapPath("/Views/Shared/register.html"));
                    var confToken = await UserMeneger.GenerateEmailConfirmationTokenAsync(user.Id);
                    var a = Url.Action("ConfirmEmail", "Auth", new {user = user.Id, token = confToken});
                    var link = $"<a href=\"http://redblackclub.ru/{a}\" target=\"_blanck\">Завершить регистрацию</a>";
                    body = body.Replace("{0}", model.Email);
                    body = body.Replace("{1}", pass);
                    body = body.Replace("{2}", link);
                    await PassAuth.SendMyMailAsync(body, model.Email, "KRASNOE & Черное - подтверждение e-mail адреса");
                }
            }
            else
            {
                var result = new[] {$"Пользователь с e-mail:{model.Email} уже зарегестрирован в нашем клубе."};
                return View("Error", result);
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                return View("Thankyou");
            }
            else
            {
               return Redirect(returnUrl);
            }
        }

        public ActionResult ConfirmEmail(string user, string token)
        {
            var result = UserMeneger.ConfirmEmail(user, token);
            return View(result);
        }

        [Authorize]
        public ActionResult LogOut()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index","Home");
        }

        public ActionResult Thankyou()
        {
            return View();
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