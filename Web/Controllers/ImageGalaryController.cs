using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Domen.Viewmodels;
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
            region.Albom = albom;
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
            //if (!ModelState.IsValid) return View(albom);
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
                    var img = ImageCrop.Crop(avatar, 500, 350, ImageCrop.AnchorPosition.Center);
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
            if (model.Category==null)
            {
                model.Category = new PatyCategory(); 
            }
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

        public ActionResult UploadsPhotos(int albomId, int regionId, string returnUrl )
        {
            ViewBag.a = albomId;
            ViewBag.r = regionId;
            ViewBag.u = returnUrl;
            return View();
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

        public ActionResult DeletePhoto(int id, string returnUrl)
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
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Redirect(returnUrl);
            }
            return RedirectToAction("AlbomDetails", new {id = albom});
        }

        [HttpPost]
        public async Task<string> UploadPhotoToGalary(int albom, int region, HttpPostedFileBase photo)
        {
            var sizeBefore = Math.Round((decimal)photo.ContentLength/1024/1024,2);
            var alb = _photo.GetGalaryById(region);
            var sizeAfter = "";
            if (photo.ContentLength>0)
            {
                if (photo.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) || photo.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) || photo.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    var img = ImageCrop.ImageScale(photo, null, 1024, 768);
                    var filename = photo.FileName.Replace(" ", "_");
                    var pathS = Server.MapPath("~/Uploads/ImageGalary/" + "_" + albom + "_" + "_" + region + "_" + filename);
                    SaveImageJpeg(pathS, img, 90L);
                    var size = new System.IO.FileInfo(pathS).Length;
                    sizeAfter = Math.Round((decimal)size / 1024 / 1024, 2).ToString();
                    var pt = new ImageData
                    {
                        FullPath = pathS,
                        Height = 768,
                        Width = 1024,
                        Path = "/Uploads/ImageGalary/" + "_" + albom + "_" + "_" + region + "_" + filename,
                        Title = photo.FileName,
                        TitleView = false,
                        Region = alb
                    };
                    await _photo.SavePhotoToRegionAsync(pt);
                }
            }

            var li = $"Фаил: {photo.FileName} размер до заливки: {sizeBefore}mb, после:{sizeAfter}mb";
            return li;
        }

        [HttpPost]
        public void BulkAddPhotos(ImageGalary region, int regId, List<HttpPostedFileBase> photos)
        {
            if (!photos.Any()) RedirectToAction("AlbomDetails", region.Albom.Id);
            region.Id = regId;
            var imgs = new List<ImageData>();
            var alb = _photo.GetGalaryById(regId);
            foreach (var data in photos.Where(data => data.FileName.EndsWith("jpg",StringComparison.OrdinalIgnoreCase) || data.FileName.EndsWith("png",StringComparison.OrdinalIgnoreCase) || data.FileName.EndsWith("jpeg",StringComparison.OrdinalIgnoreCase)))
            {

                var img = ImageCrop.ImageScale(data, null,1024, 768);
                var filename = data.FileName.Replace(" ", "_");
                var pathS = Server.MapPath("~/Uploads/ImageGalary/" + "_" + alb.Id + "_" + "_" + region.Id+"_"+ filename);
                SaveImageJpeg(pathS, img, 90L);
                //img?.Save(pathS, ImageFormat.Jpeg);
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
            //return RedirectToAction("AlbomDetails", new {id= region.Albom.Id});
        }

        [HttpPost]
        public ActionResult AddVideo(int id, string videoLink)
        {
            videoLink = videoLink.Remove(0, 17);
            var video = new ImageData
            {
                Width = 500,
                Height = 400,
                VideoLink = "https://www.youtube.com/embed/"+videoLink
            };
            var region = _photo.GetVideoRegion(id);
            video.Region = region;
            _photo.AddVideoToAlbom(video);
            return RedirectToAction("AlbomDetails", new {id=id});
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

        private void SaveImageJpeg(string path, Image image, long quality)
        {
            var qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            var jpegCodecInfo = GetCodecInfo("image/jpeg");
            if (jpegCodecInfo== null)
            {
                return;
            }
            var paramses = new EncoderParameters(1) {Param = {[0] = qualityParam}};
            image.Save(path, jpegCodecInfo, paramses);
        }
        [AllowAnonymous]
        [HttpGet]
        [OutputCache(Duration = 120, Location = OutputCacheLocation.Downstream)]
        public ActionResult GetAlbomsByCategory(int id)
        {
            var alboms = _photo.GetAlboms.Where(albom => albom.Category!=null && albom.Category.Id == id).ToList();
            return PartialView(alboms);
        }

        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}