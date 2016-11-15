using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Helpers;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class BlogController : Controller
    {
        private readonly ICmc _cmc;

        public BlogController(ICmc cmc)
        {
            _cmc = cmc;
        }
        // GET: Blog
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(_cmc.GetBlogs.OrderByDescending(blog => blog.BlogData));
        }

        public ActionResult BlogAdmin()
        {
            return View(_cmc.GetBlogs);
        }

        public ActionResult AddBlog(int id=0)
        {
            var model = new Blog
            {
                BlogData = DateTime.Now
            };
            if (id>0)
            {
                model = _cmc.GetBlogById(id);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddBlog(Blog blog, HttpPostedFileBase avatar)
        {
            var model = blog;
            if (blog.Id>0)
            {
                model = _cmc.GetBlogById(blog.Id);
                model.Title = blog.Title;
                model.BlogBody = blog.BlogBody;
                model.BlogData = blog.BlogData;
            }
            var guid = Guid.NewGuid();
            if (avatar!=null)
            {
                DeteteImage(model.FullPath);
                var filename = Server.MapPath("/Uploads/Blog/"+"_"+guid+"_"+avatar.FileName);
                var ava = ImageCrop.Crop(avatar, 500, 350, ImageCrop.AnchorPosition.Center);
                ava.Save(filename);
                model.FullPath = filename;
                model.Avatar = "/Uploads/Blog/" + "_" + guid + "_" + avatar.FileName;
            }
            _cmc.SaveBlog(model);
            return RedirectToAction("BlogAdmin");
        }

        public ActionResult DeleteBlog(int id)
        {
            var model = _cmc.GetBlogById(id);
            DeteteImage(model.FullPath);
            _cmc.DeleteBlog(id);
            return RedirectToAction("BlogAdmin");
        }

        [AllowAnonymous]
        public ActionResult BlogDetails(int id)
        {
            return View(_cmc.GetBlogById(id));
        }
        private static void DeteteImage(string file)
        {
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }
        }

    }
}