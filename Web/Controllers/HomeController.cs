using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NickBuhro.Translit;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Domen.Viewmodels;
using Web.Helpers;
using Web.Infrastructure;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHome _home;
        private readonly ICmc _cmc;
        private AppUserManager UserMeneger => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public HomeController(IHome home,ICmc cmc)
        {
            _home = home;
            _cmc = cmc;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Company()
        {
            return View();
        }
        public ActionResult Contacts()
        {
            return View();
        }
        public ActionResult Navigation()
        {
            return PartialView();
        }
        public ActionResult Header()
        {
            return PartialView();
        }
        public ActionResult Footer()
        {
            return PartialView();
        }
        public ActionResult TopSlider()
        {
            return PartialView();
        }
        public ActionResult NearPaty()
        {
            var model = _home.GetPatys.Where(paty => paty.PatyDate>=DateTime.Now && paty.PatyDate<DateTime.Today.AddMonths(1)).OrderBy(paty => paty.PatyDate);
            return PartialView(model);
        }
        public ActionResult Offer()
        {
            return PartialView();
        }
        public ActionResult Sdescr()
        {
            return PartialView();
        }
        public ActionResult PatyList()
        {
            var model = _home.GetCategorys;
            return PartialView(model);
        }
        public ActionResult Prevelegii()
        {
            return PartialView();
        }
        public ActionResult PhotoVideo()
        {
            var vasia = _home.GetPhotos.ToList();
            var model = _home.GetPhotos.Where(data => data.TitleView).Take(9).ToList();
            var video = _home.GetPhotos.Where(d => !string.IsNullOrEmpty(d.VideoLink)).ToList();
            model.AddRange(video);
            return PartialView(model);
        }
        public ActionResult Galary(int page=1)
        {
            const int itemsPerPage = 9;
            var model = new AlbomsViewmodel
            {
                Alboms = _home.GetAlboms.OrderByDescending(albom => albom.AlbomDate).Skip((page-1)*itemsPerPage).Take(itemsPerPage).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    TotalItems = _home.GetAlboms.Count(),
                    ItemsPerPage = itemsPerPage
                }
            };
            return View(model);
        }
        public ActionResult AlbomDetails(int id, int page=1, string returnUrl="")
        {
            const int itemPerPage = 30;
            var albom = _home.GetAlbomById(id);
            var model = new PhotosViewModel
            {
                Photos = _home.GetPhotos.Where(data => data.VideoLink==null && data.Region.Albom.Id==id).Skip((page-1)*itemPerPage).Take(itemPerPage).ToList(),
                PagingInfo = new PagingInfo
                {
                    TotalItems = _home.GetPhotos.Count(data => data.VideoLink == null && data.Region.Albom.Id == id),
                    ItemsPerPage = itemPerPage,
                    CurrentPage = page
                },
                Albom = albom
            };
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        public ActionResult Partners()
        {
            return PartialView();
        }
        public ActionResult RegisterPartnerPopUp()
        {
            return PartialView();
        }
        public ActionResult RegisterPopUp()
        {
            return PartialView();
        }
        public ActionResult LoginPopUp()
        {
            return PartialView();
        }
        public ActionResult ThankYou()
        {
            return PartialView();
        }
        public ActionResult NearDetails()
        {
            var model = _home.GetPatys.Where(paty => paty.PatyDate >= DateTime.Today).ToList();
            return View(model);
        }
        public ActionResult PatyDetails(string paty)
        {
            return View(_home.GetPatyByRouteUrl(paty));
        }
        public ActionResult DesktopPatyForm(int patyId)
        {
            return PartialView(_home.GetPaty(patyId));
        }
        public ActionResult MobilePatyForm(int patyId)
        {
            return PartialView(_home.GetPaty(patyId));
        }
        public ActionResult PatyForm(int patyId)
        {
            var model = new OrderViewmodel();
            if (User.Identity.IsAuthenticated)
            {
                var user =  UserMeneger.FindByName(User.Identity.Name);
                model.Customer = _home.GetCustomerByEmail(user.Email);
            }
            model.Paty = _home.GetPaty(patyId);
            return PartialView(model);
        }
        public ActionResult PatyPhotos(int id)
        {
            var model = _home.GetAlboms.Where(albom => albom.Category!=null && albom.Category.Id == id).ToList();
            var video = new List<ImageData>();
            var photos = new List<ImageData>();
            foreach (var photo in model.SelectMany(albom => albom.Regions.SelectMany(region => region.Photos.Where(data => !string.IsNullOrEmpty(data.VideoLink) || data.TitleView))))
            {
                if (!string.IsNullOrEmpty(photo.VideoLink))
                {
                    video.Add(photo);
                }
                else
                {
                    photos.Add(photo);
                }
            }
            if (!video.Any()) return PartialView(photos);
            var i = video.Max(data => data.Id);
            photos.Add(video.FirstOrDefault(data => data.Id==i));
            return PartialView(photos);
        }
        public ActionResult OrderDetails(int id)
        {
            return View(_home.GetOrderBuId(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Order(OrderViewmodel model)
        {
            var customer = await _home.GetCustomerAsync(model);
            var order = await _home.OrderExistAsync(model.PatyId, model.Email);
            if (order!=null && order.Paty.Price<=0)
            {
                var ebody = System.IO.File.ReadAllText(Server.MapPath("/Views/Shared/ticket.html"));
                var emessageBody = MakeOrderBody(ebody, order);
                await PassAuth.SendMyMailAsync(emessageBody, model.Email, "Копия приглашение на мероприятие с сайта redblackclub.ru");
                return RedirectToAction("OrderDetails", order);
            }
            decimal disc = 1;
            if (User.Identity.IsAuthenticated)
            {
                disc = disc - (decimal) 0.05;
            }
            order = await _home.RegOnPatyAsync(model.PatyId,model.Place,disc,customer);
            if (order.Paty.Price > 0)
            {
                var pay = _home.GetOrderBuId(order.Id);
                return RedirectToAction("Payment", "Order", pay);
            }
            var body = System.IO.File.ReadAllText(Server.MapPath("/Views/Shared/ticket.html"));
            var messageBody = MakeOrderBody(body, order);
            await PassAuth.SendMyMailAsync(messageBody, model.Email, "Приглашение на мероприятие с сайта redblackclub.ru");
            return RedirectToAction("OrderDetails",order);
        }
        private static string MakeOrderBody(string body, Order order)
        {
            var result = body;
            var oDate = order.OrderDate.ToLongDateString().Split(' ');
            var pDate = order.Paty.PatyDate.ToLongDateString().Split(' ');
            result = result.Replace("{0}", pDate[0]);
            result = result.Replace("{1}", pDate[1]+","+pDate[2]);
            result = result.Replace("{2}", order.Paty.PatyDate.ToShortTimeString());
            result = result.Replace("{3}", order.Paty.Title);
            result = result.Replace("{4}", order.Paty.Place);
            result = result.Replace("{5}", order.Customer.Fio);
            result = result.Replace("{6}", order.PlaceNumbers);
            result = result.Replace("{7}", order.TotalCost.ToString());
            result = result.Replace("{8}", oDate[0]+" "+oDate[1]);
            result = result.Replace("{9}", order.OrderDate.ToShortTimeString());
            result = result.Replace("{10}", order.Id.ToString());
            return result;
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Ticket()
        {
            return View();
        }
        public ActionResult Countdown(DateTime? targetDate, string text = "")
        {
            if (!string.IsNullOrEmpty(text))
            {
                ViewBag.l = Transliteration.CyrillicToLatin(text, Language.Russian);
            }
            return PartialView(ViewBag.TargetDate = DateTime.Now.AddSeconds(600));
        }

        public ActionResult Otzivi()
        {
            return View(_cmc.GetOtzivs);
        }
        public ActionResult AskMePopUp()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> AskMePopUp(string fio, string phone, string email, string message)
        {
            var body = "";

            await PassAuth.SendMyMailAsync(body, "info@redblackclub.ru", "Форма отбратной связи");

            return "Ok";
        }
    }
}