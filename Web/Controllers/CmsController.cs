﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Domen.Viewmodels;
using Web.Infrastructure;
using Web.Models;
using Web.Models.Viewmodels;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class CmsController : Controller
    {
        private readonly ICmc _repoCmc;
        private void AddErrorsFormResult(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item);
            }
        }
        private AppUserManager UserMeneger => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private AppRoleManager RoleMeneger => HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();

        public CmsController(ICmc repoCmc)
        {
            _repoCmc = repoCmc;
        }
        // GET: Cms
        public ActionResult Index()
        {
            ViewBag.active = "curLine";

            return View(UserMeneger.Users);
        }
        public ActionResult Roles()
        {
            return View(RoleMeneger.Roles);
        }

        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateRole([Required]string name)
        {
            if (!ModelState.IsValid) return View(name);
            var result = await RoleMeneger.CreateAsync(new AppRole(name));
            if (result.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            AddErrorsFormResult(result);
            return View(name);
        }

        public async Task<ActionResult> DeleteRole(string id)
        {
            var role = await RoleMeneger.FindByIdAsync(id);
            if (role != null)
            {
                var result = await RoleMeneger.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    return View("Error", (string[]) result.Errors);
                }
            }
            else
            {
                return View("Error", new[] { "Role not found" });
            }
        }

        public async Task<ActionResult> EditRole(string id)
        {
            var roel = await RoleMeneger.FindByIdAsync(id);
            var memberIds = roel.Users.Select(u => u.UserId).ToArray();
            IEnumerable<AppUser> members = UserMeneger.Users.Where(u => memberIds.Any(i => i == u.Id));
            IEnumerable<AppUser> nonMembers = UserMeneger.Users.Except(members);
            return View(new RoleEditView { Role = roel, Members = members, NonMember = nonMembers });
        }

        [HttpPost]
        public async Task<ActionResult> EditRole(RoleModModel model)
        {
            if (!ModelState.IsValid) return View("Error", new[] { "Role not found" });
            IdentityResult result;
            foreach (var toAdd in model.IdsToAdd ?? new string[] { })
            {
                result = await UserMeneger.AddToRoleAsync(toAdd, model.Name);
                if (!result.Succeeded)
                {
                    return View("Error", (string[]) result.Errors);
                }
            }
            foreach (var toRem in model.IdsToRemove ?? new string[] { })
            {
                result = await UserMeneger.RemoveFromRoleAsync(toRem, model.Name);
                if (!result.Succeeded)
                {
                    return View("Error", (string[]) result.Errors);
                }
            }
            return RedirectToAction("Roles");
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
            var user = new AppUser { UserName = model.Email, Email = model.Email };
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
            if (user == null) return View("Error", new[] { "User was not found" });
            var result = await UserMeneger.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View("Error", (string[]) result.Errors);
        }

        public async Task<ActionResult> EditUser(string id)
        {
            var user = await UserMeneger.FindByIdAsync(id);
            return user == null ? View("Error", new[] { "Something wrong" }) : View(user);
        }
        [HttpPost]
        public async Task<ActionResult> EditUser(AppUser model)
        {
            var user = await UserMeneger.FindByIdAsync(model.Id);
            if (user != null)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
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
                ModelState.AddModelError("", "User not found");
            }
            return View(model);
        }

        public ActionResult Otzivi()
        {
            return View(_repoCmc.GetOtzivs);
        }

        [HttpPost]
        public ActionResult AddVideo(string name,string company, string video)
        {
            var videoLink = video.Remove(0, 17);
            var videoData = new Otziv
            {
                VideoLink = "https://www.youtube.com/embed/" + videoLink,
                WhoSay = name,
                Work = company
            };
            _repoCmc.AddOtziv(videoData);
            return RedirectToAction("Otzivi");
        }

        public ActionResult DeleteOtziv(int id)
        {
            _repoCmc.DeleteOtziv(id);

            return RedirectToAction("Otzivi");
        }
    }
}