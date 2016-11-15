using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
        private AppUserManager UserMeneger => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        public HomeController(IHome home)
        {
            _home = home;
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
            var model = _home.GetPhotos.Where(data => data.TitleView).Take(9).ToList();
            return PartialView(model);
        }

        public ActionResult Galary(int page=1)
        {
            const int itemsPerPage = 9;
            var model = new AlbomsViewmodel
            {
                Alboms = _home.GetAlboms.OrderByDescending(albom => albom.AlbomDate).Skip((page-1)*itemsPerPage).Take(itemsPerPage),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    TotalItems = _home.GetAlboms.Count(),
                    ItemsPerPage = itemsPerPage
                }
            };
            return View(model);
        }

        public ActionResult AlbomDetails(int id, string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View(_home.GetAlbomById(id));
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

        public async Task<ActionResult> PatyDetails(string paty)
        {
            var model = new OrderViewmodel();
            if (User.Identity.IsAuthenticated)
            {
                var user = await UserMeneger.FindByNameAsync(User.Identity.Name);
                model.Customer = await _home.GetCustomerByEmailAsync(user.Email);
            }
            model.Paty = _home.GetPatyByRouteUrl(paty);

            return View(model);
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

        private string MakeOrderBody(string body, Order order)
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
     }
}