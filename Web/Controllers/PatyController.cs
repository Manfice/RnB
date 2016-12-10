using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Newtonsoft.Json;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Domen.Viewmodels;
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

        public ActionResult PatyInner(int id)
        {
            var i = _repository.GetCategoryById(id);
            var b =
                _repository.GetCategorys.Where(category => category.ParentCategory != null && category.ParentCategory.Id == id)
                    .ToList();
            var model = new CategoryViewModel
            {
                Category = i,
                Categories = b
            };
            return View(model);
        }
        public JsonResult GetCategorys()
        {
            var result = _repository.GetCategorys;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPatys()
        {
            var model = _repository.GetPatys.Select(p => new
            {
                p.PatyDate,p.Avatar,p.Descr,p.Title,p.AddRate,p.Category, p.Dres,p.Id,p.MaxGuests,p.PatyInterest,p.Place,p.Price, ord = p.Orders.Select(order => new
                {
                    order.Id,
                    order.Place,
                    order.PlaceNumbers,

                    order.Customer.Fio,
                    order.Customer.Email,
                    order.Customer.Phone
                }),
                UsedPlaceCount = p.Orders.Sum(order => order.Place)
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCategory(int id=0, int parent=0)
        {
            var model = new PatyCategory {ParentCategory = new PatyCategory(), Avatar = new PatyImage()};
            if (id>0)
            {
                model = _repository.GetCategoryById(id);
                if (model.ParentCategory==null)
                {
                    model.ParentCategory= new PatyCategory();
                }
                if (model.Avatar==null)
                {
                    model.Avatar = new PatyImage();
                }
            }
            if (parent>0)
            {
                model.ParentCategory = _repository.GetCategoryById(parent);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddCategory(PatyCategory model, HttpPostedFileBase avatar, int parent = 0)
        {
            if (model.RouteTitle == string.Empty || _repository.CheckPatyCategoryUrlTitle(model.RouteTitle, model.Id))
            {
                ModelState.AddModelError(model.RouteTitle,"Не указан URL для события, или такой URL уже существует");
                return View(model);
            }
            var cat = model;
            if (model.Id>0)
            {
                cat = _repository.GetCategoryById(model.Id);
                cat.Title = model.Title;
                cat.RouteTitle = model.RouteTitle;
                cat.MetaDescription = model.MetaDescription;
                cat.Description = model.Description;
            }
            var guid = Guid.NewGuid();
            PatyImage image = null;
            if (avatar!=null)
            {
                if (model.Avatar!=null)
                {
                    if (System.IO.File.Exists(model.Avatar.FullPath))
                    {
                        System.IO.File.Delete(model.Avatar.FullPath);
                    }
                }
                var ava = ImageCrop.Crop(avatar, 500, 350, ImageCrop.AnchorPosition.Center);
                var filePath = Server.MapPath("/Uploads/patyCategorys/" + guid + "_sep_");
                ava.Save(filePath+avatar.FileName);
                image = new PatyImage
                {
                    ContentLength = avatar.ContentLength,
                    ContentType = avatar.ContentType,
                    FullPath = filePath+avatar.FileName,
                    Path = "/Uploads/patyCategorys/" + guid + "_sep_"+avatar.FileName
                };
            }
            var data = _repository.SavePatyCategory(parent, cat, image);
            return data.ParentCategory==null ? RedirectToAction("Index") : RedirectToAction("PatyInner", new {id=data.ParentCategory.Id});
        }

        public ActionResult CategoryPatys(int id)
        {
            var model = _repository.GetCategoryById(id);
            return View(model);
        }

        public async Task<ActionResult> AddPaty(int id = 0, int cat = 0)
        {
            if (cat == 0)
            {
                return RedirectToAction("PatyError",new {error="Не выбрано ни одной категории"});
            }
            var paty = new Paty
            {
                Category = _repository.GetCategoryById(cat),
                PatyDate = DateTime.Now,
                MaxGuests = 1,
                AddRate = 1,
                Price = 0
            };
            if (id>0)
            {
                paty = await _repository.GetPatyByIdAsync(id);
            }
            return View(paty);

        }

        public async Task<ActionResult> PrintOrders(int id)
        {
            var model = await _repository.GetPatyByIdAsync(id);
            return View(model);
        }
        public async Task<ActionResult> PatyDetails(int id)
        {
            var model = await _repository.GetPatyByIdAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> AddPaty(Paty paty, HttpPostedFileBase avatar)
        {
            if (!string.IsNullOrEmpty(paty.RouteTitle) && _repository.CheckPatyUrlTitle(paty.RouteTitle, paty.Id))
            {
                ModelState.AddModelError(paty.RouteTitle, "Не указан URL для события, или такой URL уже существует");
                return View(paty);
            }
            var guid = Guid.NewGuid();
            var filePath = Server.MapPath("/Uploads/patyAvatar/" +guid+ "_sep_");
            var model = paty;
            PatyImage image = null;
            if (paty.Id > 0)
            {
                model = await _repository.GetPatyByIdAsync(paty.Id);
                model.Dres = paty.Dres;
                model.AddRate = paty.AddRate;
                model.Address = paty.Address;
                model.Descr = paty.Descr;
                model.MaxGuests = paty.MaxGuests;
                model.PatyDate = paty.PatyDate;
                model.PatyInterest = paty.PatyInterest;
                model.Place = paty.Place;
                model.Price = paty.Price;
                model.Title = paty.Title;
                model.RouteTitle = paty.RouteTitle;
                model.MetaDescription = paty.MetaDescription;
            }
            if (model.Orders!=null && model.Orders.Any())
            {
                ModelState.AddModelError("","У события есть заказы, нельзя изменять событие!");
                return View(model);
            }
            if (avatar!=null)
            {
                if (model.Avatar!=null)
                {
                    DeteteImage(model.Avatar.FullPath);
                }
                var im = ImageCrop.Crop(avatar, 970, 679, ImageCrop.AnchorPosition.Center);
                im.Save(filePath+avatar.FileName);
                image = new PatyImage
                {
                    ContentLength = avatar.ContentLength,
                    ContentType = avatar.ContentType,
                    FullPath = filePath+avatar.FileName,
                    Path = "/Uploads/patyAvatar/" + guid + "_sep_"+avatar.FileName
                };
            }
            var c = model.Avatar?.Id ?? 0;
            var data = await _repository.AddPatyAsync(c, model, image);
            if (data.Success)
            {
                return RedirectToAction("CategoryPatys", new {id = data.Paty.Category.Id});
            }
            return View(paty);
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
            var filePath = Server.MapPath("/Uploads/patyCategorys/" + guid + "_sep_");
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
                    var photo = ImageCrop.Crop(logo, 500, 350, ImageCrop.AnchorPosition.Center);
                    if (photo != null)
                    {
                        filePath += logo.FileName;
                        try
                        {
                            using (var m = new MemoryStream())
                            {
                                using (var f = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                                {
                                    photo.Save(m, ImageFormat.Jpeg);
                                    var bytes = m.ToArray();
                                    f.Write(bytes,0,bytes.Length);
                                }
                            }
                            img = new PatyImage
                            {
                                ContentType = logo.ContentType,
                                FullPath = filePath,
                                Path = "/Uploads/patyCategorys/" + guid + "_sep_" + logo.FileName
                            };
                        }
                        catch (Exception)
                        {
                            img = null;
                        }
                    }
                }
            }

            data.Id = currId;
            var result = await _repository.AddCategoryAsync(parent,avaId,data, img);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public async Task<JsonResult> DeleteCategory(int id)
        //{
        //    var photos = await _repository.GetImagesAsync(id);

        //    var result = await _repository.DeleteCategoryAsync(id);

        //    foreach (var item in photos.Where(item => System.IO.File.Exists(item.FullPath)))
        //    {
        //        System.IO.File.Delete(item.FullPath);
        //    }

        //     _repository.DeleteImagesAsync(photos);

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public async Task<ActionResult> DeleteCategory(int id, string returnUrl)
        {
            var category = _repository.GetCategoryById(id);
            var error = "";
            var i = category.ParentCategory;
            if (category.Paties.Any())
            {
                error="Нельзя удалить категорию, в которой есть мероприятия";
            }
            if (_repository.GetCategorys.Any(patyCategory => patyCategory.ParentCategory!=null && patyCategory.ParentCategory.Id==id))
            {
                error = "Категория содержит подкатегорию, удалите сначала ее!";
            }
            if (!string.IsNullOrEmpty(error))
            {
                return RedirectToAction("PatyError", new {error, returnUrl});
            }
            if (category.Avatar!=null)
            {
                DeteteImage(category.Avatar.FullPath);
            }
            await _repository.DeleteCategoryAsync(category.Id);
            return i == null ? RedirectToAction("Index") : RedirectToAction("PatyInner", new {id=i.Id});
        }

        public async Task<ActionResult> DeletePaty(int id)
        {
            var model = await _repository.GetPatyByIdAsync(id);
            if (model==null)
            {
                return RedirectToAction("PatyError", new {error = "Мероприятие нет"});
            }
            var c = model.Category.Id;
            if (model.Avatar!=null)
            {
                DeteteImage(model.Avatar.FullPath);
            }
            var data = await _repository.DeletePatyAsync(id);
            if (data.Success)
            {
                return RedirectToAction("CategoryPatys", new {id=c});
            }
            return RedirectToAction("PatyError", new {error = data.Errors[0]});
            
        } 
        public ActionResult PatyError(string error, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.error = error;
            return View();
        }
        //[HttpPost]
        //public async Task<JsonResult> AddPatyAsync()
        //{
        //    var paty = new Paty();
        //    var guid = Guid.NewGuid();
        //    var catId = 0;
        //    var avaId = 0;
        //    var filePath = Server.MapPath("~/Uploads/patyAvatar/" +guid+ "_sep_");
        //    PatyImage img = null;
            
        //    var formData = HttpContext.Request.Form.AllKeys;
        //    if (formData.Length > 0)
        //    {
        //        paty.Id = int.Parse(HttpContext.Request.Form["id"]);//1
        //        paty.PatyDate = DateTime.Parse(HttpContext.Request.Form["date"]);//2
        //        paty.Title = HttpContext.Request.Form["title"];//3
        //        paty.Descr = HttpContext.Request.Form["descr"];//4
        //        paty.MaxGuests = int.Parse(HttpContext.Request.Form["maxGuests"]);//5
        //        paty.Price = decimal.Parse(HttpContext.Request.Form["Price"]);//6
        //        paty.PatyInterest = HttpContext.Request.Form["interest"];//7
        //        paty.AddRate = int.Parse(HttpContext.Request.Form["rate"]);//8
        //        paty.Dres = HttpContext.Request.Form["dres"];//9
        //        paty.Place = HttpContext.Request.Form["place"];//10

        //        int.TryParse(HttpContext.Request.Form["category"], out catId);//13
        //        int.TryParse(HttpContext.Request.Form["avaId"], out avaId);//14
        //    }
        //    if (HttpContext.Request.Files.Count > 0)
        //    {
        //        var logo = HttpContext.Request.Files["Avatar"];
        //        if (logo != null && logo.ContentLength > 0)
        //        {
        //            //Todo: перевести в отдельную процедуру
        //            if (avaId > 0)
        //            {
        //                var fileToDelete = await _repository.ImgToDeleteAsync(avaId);
        //                if (fileToDelete != null)
        //                {
        //                    if (System.IO.File.Exists(fileToDelete.FullPath))
        //                    {
        //                        System.IO.File.Delete(fileToDelete.FullPath);
        //                    }
        //                }
        //            }
        //            var photo = ImageCrop.Crop(logo, 500, 350, ImageCrop.AnchorPosition.Center);
        //            if (photo != null)
        //            {
        //                filePath += logo.FileName;
        //                photo.Save(filePath, ImageFormat.Jpeg);
        //                img = new PatyImage
        //                {
        //                    ContentType = logo.ContentType,
        //                    FullPath = filePath,
        //                    Path = "/Uploads/patyAvatar/" + guid + "_sep_" + logo.FileName
        //                };
        //            }
        //        }
        //    }

        //    var result = await _repository.AddPatyAsync(catId, avaId, paty, img);

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //public async Task<JsonResult> RemovePaty(int id)
        //{
        //    var paty = await _repository.GetPatyByIdAsync(id);

        //    if (!paty.Success) return Json(paty, JsonRequestBehavior.AllowGet);

        //    if (paty.Paty.Orders.Any())
        //    {
        //        paty.Success = false;
        //        paty.Errors = new[] {"Нельзя удалить мероприятие на которое уже проданы места!"};
        //        return Json(paty, JsonRequestBehavior.AllowGet);
        //    }

        //    if (paty.Paty.Avatar!=null)
        //    {
        //        if (System.IO.File.Exists(paty.Paty.Avatar.FullPath))
        //        {
        //            System.IO.File.Delete(paty.Paty.Avatar.FullPath);
        //        }
        //    }

        //    paty = await _repository.DeletePatyAsync(paty.Paty.Id);

        //    return Json(paty, JsonRequestBehavior.AllowGet);
        //}

        [AllowAnonymous]
        public ActionResult Paty()
        {
            return View(_repository.GetCategorys);
        }
        [AllowAnonymous]
        public ActionResult PatyMeny()
        {
            return PartialView(_repository.GetCategorys);
        }
        [AllowAnonymous]
        public ActionResult PatyMenuDetails(string patycat, int id = 0)
        {
            var askCategory = _repository.GetCategorys.FirstOrDefault(category => category.RouteTitle.Contains(patycat));
            if (askCategory == null)
            {
                return RedirectToAction("Paty");
            }
            var model = new CategoryViewModel
            {
                Category = askCategory,
                Categories = _repository.GetCategorys.Where(category => category.ParentCategory != null && category.ParentCategory.Id == askCategory.Id).ToList()
            };
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult CategoryDetails(string patyinner)
        {
            var model = _repository.GetCategoryByRoute(patyinner);
            return View(model);
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
