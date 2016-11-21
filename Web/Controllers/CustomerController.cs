using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Domen.Viewmodels;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly string _user = System.Web.HttpContext.Current.User.Identity.GetUserId();
        private readonly string _userName = System.Web.HttpContext.Current.User.Identity.GetUserName();

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
            if (customer == null)
            {
                var user = UserManager.FindById(_user);
                var newCustomer = new Customer
                {
                    User = user.Id,
                    Email = user.Email
                };
                customer = _customer.CreateCustomerByUser(newCustomer);
            }
            else
            {
                customer = _customer.CreateCustomerByUser(customer);
            }
            if (customer.Avatar==null)
            {
                customer.Avatar = new CustImage();
            }

            return View(customer);
        }

        public ActionResult MyVisitedPatyes()
        {
            return PartialView(_customer.GetVisitedPaties(_user));
        }

        public ActionResult SuggestedPatyes()
        {
            return PartialView(_customer.GetSuggestedPaties(_user));
        }

        public ActionResult PersonalDetails()
        {
            var customer = _customer.GetCustomerByUserId(_user);
            if (customer==null)
            {
                return RedirectToAction("Index");
            }
            if (customer.Avatar == null)
            {
                customer.Avatar = new CustImage();
            }

            return View(CompareCustomerVmLk(customer));
        }
        public ActionResult Company()
        {
            var customer = _customer.GetCustomerByUserId(_user);
            if (customer.Avatar == null)
            {
                customer.Avatar = new CustImage();
            }
            if (customer.Company==null)
            {
                customer.Company = new Company();
            }

            return View(customer);
        }

        public ActionResult MyOrders()
        {
            return View(_customer.GetMyOrders(_user));
        }
        [HttpPost]
        public ActionResult PersonalDetails(CustomerVmLk model)
        {
            var customer = _customer.GetCustomerByUserId(_user);
            customer.ShowData = ShowData(model);
            customer.Fio = model.Customer.Fio;
            customer.Phone = model.Customer.Phone;
            customer.Email = model.Customer.Email;
            customer.City = model.Customer.City;
            customer.Birthday = model.Customer.Birthday;
            customer.WorkPlace = model.Customer.WorkPlace;
            customer.Facebook = model.Customer.Facebook;
            customer.Insta = model.Customer.Insta;
            customer.Vk = model.Customer.Vk;
            customer.Od = model.Customer.Od;
            customer.Twit = model.Customer.Twit;
            customer.AboutMe = model.Customer.AboutMe;
            customer.EmailNotice = model.Customer.EmailNotice;
            customer.SmsNotice = model.Customer.SmsNotice;
            customer.PhoneNotice = model.Customer.PhoneNotice;
            if (!string.IsNullOrEmpty(model.OldPass))
            {
                var user = UserManager.Find(_userName, model.OldPass);
                if (user == null)
                {
                    ModelState.AddModelError(model.OldPass, "Не верно указан действующий пароль!");
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.NewPass) && !string.IsNullOrEmpty(model.Confirm))
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPass);
                    }
                    else
                    {
                        ModelState.AddModelError(model.NewPass, "Новый пароль должен содержать не менее одного символа.");
                    }
                }
            }
            if (ModelState.Values.SelectMany(state => state.Errors).Any())
            {
                return View(CompareCustomerVmLk(customer));
            }
            _customer.UpdateCustomer(customer);
            return View(CompareCustomerVmLk(customer));
        }

        [HttpPost]
        public ActionResult SaveMyCompany(Company model)
        {
            _customer.UpdateMyCompany(model);
            return RedirectToAction("Company");
        }
        [HttpPost]
        public ActionResult SaveAvatar(HttpPostedFileBase avatar)
        {
            var customer = _customer.GetCustomerByUserId(_user);
            if (avatar == null || avatar.ContentLength <= 0) return RedirectToAction("Index");
            var filePath = Server.MapPath("/Uploads/Partners/" + _user + "_" + avatar.FileName);
            avatar.SaveAs(filePath);
            if (customer.Avatar!=null)
            {
                if (System.IO.File.Exists(customer.Avatar.FullPath))
                {
                    System.IO.File.Delete(customer.Avatar.FullPath);
                }
            }
            var ava = new CustImage
            {
                ContentType = avatar.ContentType,
                Path = "/Uploads/Partners/" + _user + "_" + avatar.FileName,
                FullPath = filePath
            };
            _customer.SetAvatar(customer.Id, ava);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SaveCompanyAvatar(int id, HttpPostedFileBase avatar)
        {
            var customer = _customer.GetCustomerByUserId(_user);
            if (avatar == null || avatar.ContentLength <= 0) return RedirectToAction("Company");
            var filePath = Server.MapPath("/Uploads/Partners/" + _user + "_comp_" + avatar.FileName);
            avatar.SaveAs(filePath);
            if (customer.Company.Image != null)
            {
                if (System.IO.File.Exists(customer.Company.Image.FullPath))
                {
                    System.IO.File.Delete(customer.Company.Image.FullPath);
                }
            }
            var ava = new CustImage
            {
                ContentType = avatar.ContentType,
                Path = "/Uploads/Partners/" + _user + "_comp_" + avatar.FileName,
                FullPath = filePath
            };
            _customer.SetCompanyAvatar(id, ava);

            return RedirectToAction("Company");
        }
        private static string ShowData(CustomerVmLk model)
        {
            var s = "";
            s += model.Fio ? "1" : "0";
            s += model.Phone ? "1" : "0";
            s += model.Mail ? "1" : "0";
            s += model.City ? "1" : "0";
            s += model.Birthday ? "1" : "0";
            s += model.WorkPlace ? "1" : "0";
            s += model.F ? "1" : "0";
            s += model.I ? "1" : "0";
            s += model.V ? "1" : "0";
            s += model.O ? "1" : "0";
            s += model.T ? "1" : "0";
            s += model.Sex;
            return s;
        }
        private static CustomerVmLk CompareCustomerVmLk(Customer customer)
        {
            var result = new CustomerVmLk
            {
                Customer = customer,
                Fio = customer.ShowData[0].Equals('1'),
                Phone = customer.ShowData[1].Equals('1'),
                Mail = customer.ShowData[2].Equals('1'),
                City = customer.ShowData[3].Equals('1'),
                Birthday = customer.ShowData[4].Equals('1'),
                WorkPlace = customer.ShowData[5].Equals('1'),
                F = customer.ShowData[6].Equals('1'),
                I = customer.ShowData[7].Equals('1'),
                V = customer.ShowData[8].Equals('1'),
                O = customer.ShowData[9].Equals('1'),
                T = customer.ShowData[10].Equals('1'),
                Sex = customer.ShowData[11].ToString()
            };
            return result;
        }
    }
}