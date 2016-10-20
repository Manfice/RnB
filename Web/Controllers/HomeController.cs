﻿using System.Linq;
using System.Web.Mvc;
using Web.Domen.Abstract;

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
            return PartialView();
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

    }
}