using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Domen.Viewmodels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private AppUserManager UserMeneger => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        // GET: Auth
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.active = "curLine";

            return View(UserMeneger.Users);
        }

        public ActionResult CreateUser()
        {
            ViewBag.active = "curLine";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateUser(UserView model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new AppUser {UserName = model.Email, Fio = model.Name, Email = model.Email};
            var result = await UserMeneger.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            AddErrorsFormResult(result);
            return View(model);
        }

        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserMeneger.FindByIdAsync(id);
            if (user == null) return View("Error", new string[] {"User was not found"});
            var result = await UserMeneger.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View("Error", result.Errors);
        }

        public async Task<ActionResult> EditUser(string id)
        {
            var user = await UserMeneger.FindByIdAsync(id);
            return user == null ? View("Error", new string[] { "Something wrong" }) : View(user);
        }
        [HttpPost]
        public async Task<ActionResult> EditUser(AppUser model)
        {
            var user = await UserMeneger.FindByIdAsync(model.Id);
            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Fio = model.Fio;
                var validEmail = await UserMeneger.UserValidator.ValidateAsync(user);
                if (!validEmail.Succeeded)
                {
                    AddErrorsFormResult(validEmail);
                }
                else
                {
                    var result = await UserMeneger.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                       return RedirectToAction("Index");
                    }
                }
                
            }
            else
            {
                ModelState.AddModelError("","User not found");
            }
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.Request.Url == null) return View();
            var fromUrl = HttpContext.Request.Url.PathAndQuery;
            ViewBag.fromUrl = fromUrl;
            return View();
        }

        private void AddErrorsFormResult(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("",item);
            }
        }

    }
}