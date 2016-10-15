using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Domen.Abstract;
using Web.Domen.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Web.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class PatyController : Controller
    {
        private readonly IEvents _repository;

        private static void MakeFile298X258(string filepath)
        {

        }
        
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
            PatyImage img = null;
            var filePath = Server.MapPath("~/Uploads/patyCategorys/" + guid + "_sep_");
            var formData = HttpContext.Request.Form.AllKeys;
            if (formData.Length>0)
            {
                data.Title = HttpContext.Request.Form["title"];
                data.Description = HttpContext.Request.Form["descr"];
            }
            if (HttpContext.Request.Files.Count>0)
            {
                var logo = HttpContext.Request.Files["UploadedImage"];
                filePath += logo?.FileName;
                logo?.SaveAs(filePath);
                MakeFile298X258(filePath);
                img = new PatyImage
                {
                    ContentType = logo?.ContentType,
                    FullPath = filePath,
                    Path = "/Uploads/patyCategorys/" + guid + "_sep_" + logo?.FileName
                };
            }
            var result = await _repository.AddCategoryAsync(data, img);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}