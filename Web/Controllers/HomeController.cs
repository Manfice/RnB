using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
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
            return PartialView();
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