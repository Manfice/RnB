using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Helpers;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class ImageGalaryController : Controller
    {
        private readonly IPhoto _photo;

        public ImageGalaryController(IPhoto photo)
        {
            _photo = photo;
        }
        // GET: ImageGalary
        public ActionResult Index()
        {
            return View(_photo.GetAlboms.OrderByDescending(albom => albom.Id));
        }

        public ActionResult CreateAlbom(int id=0)
        {
            var model = new PhotoAlbom
            {
                AlbomDate = DateTime.Now,
                Category = new PatyCategory()
            };
            if (id == 0) return View(model);
            var patyEvent = _photo.GetPatyById(id);
            model.Title = patyEvent.Title;
            model.AlbomDate = patyEvent.PatyDate;
            model.Category = patyEvent.Category;
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateRegion(ImageGalary region)
        {
            var albom = _photo.GetAlbomById(region.Albom.Id);
            _photo.SaveRegion(region);
            return RedirectToAction("AlbomDetails", new {id = albom.Id});
        }

        [HttpPost]
        public ActionResult CreateAlbom(PhotoAlbom albom, HttpPostedFileBase avatar)
        {

            if (albom.AlbomDate.Year<2010)
            {
                ModelState.AddModelError("date","Дата не должна быть меньше 01 января 2010г.");
                return View(albom);
            }
            if (!ModelState.IsValid) return View(albom);
            if (avatar!=null)
            {
                if (avatar.FileName.EndsWith("jpg",StringComparison.OrdinalIgnoreCase) || avatar.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) || avatar.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    if (albom.AvatarFullPath != null)
                    {
                        if (System.IO.File.Exists(albom.AvatarFullPath))
                        {
                            System.IO.File.Delete(albom.AvatarFullPath);
                        }
                    }
                    var guid = Guid.NewGuid();
                    var img = ImageCrop.Crop(avatar, 210, 174, ImageCrop.AnchorPosition.Center);
                    var filename = avatar.FileName.Replace(" ", "_");
                    var path = Server.MapPath("~/Uploads/ImageGalary/" + "_" + guid + "_" + filename);
                    img?.Save(path, ImageFormat.Jpeg);
                    albom.Avatar = "/Uploads/ImageGalary/" + "_" + guid + "_" + filename;
                    albom.AvatarFullPath = path;
                }
            }
            _photo.SaveAlbom(albom);
            return RedirectToAction("Index");
        }

        public ActionResult EditAlbom(int id)
        {
            var model = _photo.GetAlbomById(id);
            return View("CreateAlbom", model);
        }

        public ActionResult DeleteAlbom(int id)
        {
            var albom = _photo.GetAlbomById(id);
            var childs = _photo.GetChildElements(albom.Id);
            var paths = childs as IList<string> ?? childs.ToList();
            if (paths.Any())
            {
                foreach (var path in paths.Where(System.IO.File.Exists))
                {
                    System.IO.File.Delete(path);
                }
            }
            if (System.IO.File.Exists(albom.AvatarFullPath))
            {
                System.IO.File.Delete(albom.AvatarFullPath);
            }
            _photo.DeleteAlbom(albom.Id);
            return RedirectToAction("Index");
        }

        public ActionResult AlbomDetails(int id)
        {
            var albom = _photo.GetAlbomById(id);
            return View(albom);
        }
        public ActionResult PhotoDetails(int id)
        {
            var albom = _photo.Photo(id);
            return View(albom);
        }

        public ActionResult OnTitlePage(int id)
        {
            var model = _photo.ShowOnTitlePage(id);
            var albom = model.Region.Albom.Id;
            return RedirectToAction("AlbomDetails", new { id = albom });
        }

        public ActionResult DeletePhoto(int id)
        {
            var photo = _photo.Photo(id);
            var albom = photo.Region.Albom.Id;
            if (!string.IsNullOrEmpty(photo.FullPath))
            {
                if (System.IO.File.Exists(photo.FullPath))
                {
                    System.IO.File.Delete(photo.FullPath);
                }
            }
            _photo.DeletePhoto(id);
            return RedirectToAction("AlbomDetails", new {id = albom});
        }

        [HttpPost]
        public ActionResult BulkAddPhotos(ImageGalary region, int regId, List<HttpPostedFileBase> photos)
        {
            if (!photos.Any()) RedirectToAction("AlbomDetails", region.Albom.Id);
            region.Id = regId;
            var imgs = new List<ImageData>();
            var alb = _photo.GetGalaryById(regId);
            foreach (var data in photos.Where(data => data.FileName.EndsWith("jpg",StringComparison.OrdinalIgnoreCase) || data.FileName.EndsWith("png",StringComparison.OrdinalIgnoreCase) || data.FileName.EndsWith("jpeg",StringComparison.OrdinalIgnoreCase)))
            {

                var img = ImageCrop.Crop(data, 1024, 768, ImageCrop.AnchorPosition.Center);
                var filename = data.FileName.Replace(" ", "_");
                var pathS = Server.MapPath("~/Uploads/ImageGalary/" + "_" + alb.Id + "_" + "_" + region.Id+"_"+ filename);
                img?.Save(pathS, ImageFormat.Jpeg);
                var pt = new ImageData
                {
                    FullPath = pathS,
                    Height = 768,
                    Width = 1024,
                    Path = "/Uploads/ImageGalary/" + "_" + alb.Id + "_" + "_" + region.Id + "_" + filename,
                    Title = data.FileName,
                    TitleView = false,
                    Region = alb
                };
                imgs.Add(pt);
            }
            _photo.BulkAddPhotos(imgs);
            return RedirectToAction("AlbomDetails", new {id= region.Albom.Id});
        }

        public ActionResult EditRegion(int id)
        {
            return View(_photo.GetGalaryById(id));
        }

        [HttpPost]
        public ActionResult EditRegion(ImageGalary model)
        {
            if (string.IsNullOrEmpty(model.Title))
            {
                ModelState.AddModelError("title", "Наименование не должно быть пустым");
                return View(model);
            }
            var data = _photo.SaveRegion(model);
            return RedirectToAction("AlbomDetails", new {id=data.Albom.Id});
        }

        public ActionResult DeleteRegion(int id)
        {
            var albom = _photo.GetGalaryById(id).Albom.Id;
            var photos = _photo.GetPhotoPath(id);
            var items = photos as IList<string> ?? photos.ToList();
            if (items.Any())
            {
                foreach (var item in items.Where(System.IO.File.Exists))
                {
                    System.IO.File.Delete(item);
                }
            }
            _photo.DeleteRegion(id);
            return RedirectToAction("AlbomDetails", new {id=albom});
        }
    }
}