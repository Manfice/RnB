using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Domen.Abstract;
using Web.Domen.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHome _home;

        public HomeController(IHome home)
        {
            _home = home;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TopSlider()
        {
            return PartialView();
        }

        public ActionResult NearPaty()
        {
            var model = _home.GetPatys.OrderBy(paty => paty.PatyDate);
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
            return PartialView();
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

        public ActionResult PatyDetails(int id)
        {
            var model = _home.GetPaty(id);
            return View(model);
        }

        public async Task<ActionResult> Order(OrderViewmodel model)
        {
            var customer = await _home.GetCustomerAsync(model);
            var order = await _home.RegOnPatyAsync(model.Paty, customer);
            return View(order);
        }

     }
}