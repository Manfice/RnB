using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Helpers;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class PatyController : Controller
    {
        private readonly IEvents _repository;
        
        public PatyController(IEvents repository)
        {
            _repository = repository;
        }
        // GET: Paty
        public ActionResult Index()
        {
            return View(_repository.GetCategorys);
        }

        public JsonResult GetCategorys()
        {
            var result = _repository.GetCategorys;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> AddRootCat()
        {
            var guid = Guid.NewGuid();
            var data = new PatyCategory();
            var parent = 0;
            var currId = 0;
            var avaId = 0;
            PatyImage img = null;
            var filePath = Server.MapPath("~/Uploads/patyCategorys/" + guid + "_sep_");
            var formData = HttpContext.Request.Form.AllKeys;
            if (formData.Length>0)
            {
                data.Title = HttpContext.Request.Form["title"];
                data.Description = HttpContext.Request.Form["descr"];
                int.TryParse(HttpContext.Request.Form["ParentId"], out parent);
                int.TryParse(HttpContext.Request.Form["id"], out currId);
                int.TryParse(HttpContext.Request.Form["avaId"], out avaId);
            }
            if (HttpContext.Request.Files.Count>0)
            {
                var logo = HttpContext.Request.Files["UploadedImage"];
                if (logo!=null && logo.ContentLength>0)
                {
                    if (avaId>0)
                    {
                        var fileToDelete = await _repository.ImgToDeleteAsync(avaId);
                        if (fileToDelete!=null)
                        {
                            if (System.IO.File.Exists(fileToDelete.FullPath))
                            {
                                System.IO.File.Delete(fileToDelete.FullPath);
                            }
                        }
                    }
                    var photo = ImageCrop.Crop(logo, 298, 258, ImageCrop.AnchorPosition.Center);
                    if (photo != null)
                    {
                        filePath += logo.FileName;
                        photo.Save(filePath, ImageFormat.Jpeg);
                        img = new PatyImage
                        {
                            ContentType = logo.ContentType,
                            FullPath = filePath,
                            Path = "/Uploads/patyCategorys/" + guid + "_sep_"+logo.FileName
                        };

                    }
                }
            }

            data.Id = currId;
            var result = await _repository.AddCategoryAsync(parent,avaId,data, img);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteCategory(int id)
        {
            var photos = await _repository.GetImagesAsync(id);

            var result = await _repository.DeleteCategoryAsync(id);

            foreach (var item in photos.Where(item => System.IO.File.Exists(item.FullPath)))
            {
                System.IO.File.Delete(item.FullPath);
            }

             _repository.DeleteImagesAsync(photos);

            return Json(result, JsonRequestBehavior.AllowGet);
        } 
    }
}