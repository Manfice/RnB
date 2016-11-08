using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Web.Domen.Abstract;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly string _user = System.Web.HttpContext.Current.User.Identity.GetUserId();

        private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

        private readonly ICustomer _customer;

        public CustomerController(ICustomer customer)
        {
            _customer = customer;
        }

        // GET: Customer
        public ActionResult Index()
        {
            var customer = _customer.GetCustomerByUserId(_user);

            return View(customer);
        }
    }
}