using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Web.Domen.Abstract;
using Web.Domen.Models;
using Web.Domen.Viewmodels;
using Web.Helpers;
using Web.Infrastructure;
using Web.Models;
using Web.Models.Viewmodels;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private AppUserManager UserMeneger => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
        private AppSignInManager SignInManager => HttpContext.GetOwinContext().Get<AppSignInManager>();
        private AppRoleManager RoleMeneger => HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;
        private readonly IAuth _auth;
        private readonly ICustomer _customer;

        public AuthController(IAuth auth,ICustomer customer)
        {
            _auth = auth;
            _customer = customer;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.fromUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LogivVm model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserMeneger.FindAsync(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Ошибка логина или пароля");
            }
            else
            {
                if (!user.EmailConfirmed)
                {
                    await SendEmailConfirmationAsync(user);
                    ModelState.AddModelError("","Вам нужно подтвердить ваш e-mail. Проверьте почту.");
                    return View(model);
                }
                if (!_auth.CheckCustomerExist(user.Id))
                {
                    var cst = new Customer
                    {
                        User = user.Id,
                        Email = user.Email,
                        ShowData = "00000000000N",
                        Avatar = new CustImage(),
                        Birthday = DateTime.Parse("01.01.1980"),
                        Company = new Company(),
                        EmailNotice = true,
                        PhoneNotice = true,
                        SmsNotice = true,
                        Rate = 0,
                    };
                    _customer.UpdateCustomer(cst);
                }
                var ident = await UserMeneger.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthManager.SignOut();
                AuthManager.SignIn(new AuthenticationProperties {IsPersistent = true},ident);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            ViewBag.fromUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback","Auth", new {returnUrl}));
        }

        [AllowAnonymous]
        public ActionResult VkAuth()
        {
            const string appId = "5749182";
            const string scope = "email";
            var redirectUri = "https://redblackclub.ru" + Url.Action("GetVkCodeCallback");
            return Redirect($"https://oauth.vk.com/authorize?client_id={appId}&display=popup&redirect_uri={redirectUri}&scope={scope}&v=5.60");
        }

        public async Task<ActionResult> GetVkCodeCallback(string code, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return RedirectToAction("ExternalLoginFailure", "Auth");
            }

            var accessData = VkAuthClient.GetToken(code);

            if (accessData==null)
            {
                return RedirectToAction("ExternalLoginFailure", "Auth");
            }

            var userDetails = VkAuthClient.GetUserDetails(accessData);
            userDetails.email = accessData.email;
            var user = await UserMeneger.FindByEmailAsync(userDetails.email);
            if (user != null)
            {
                if (!user.EmailConfirmed) await SendEmailConfirmationAsync(user);
                SignInManager.SignIn(user, isPersistent: true, rememberBrowser: true);
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                return View("VkLoginConfirmation", userDetails);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> VkLoginConfirmation(VkUserInfo model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Customer");
            }
            if (!ModelState.IsValid) return View(model);
            var user = await UserMeneger.FindByEmailAsync(model.email);
            if (user!=null)
            {
                ModelState.AddModelError(model.email, "Пользователь с таким E-mail уже зарегистрирован в системе. Если вы не помните свой пароль, воспользуйтесь функцией восстановления пароля.");
            }
            user = new AppUser
            {
                UserName = model.email,
                Email = model.email
            };
            var result = await UserMeneger.CreateAsync(user);
            if (result.Succeeded)
            {
                user = await UserMeneger.FindByEmailAsync(model.email);
                result = await UserMeneger.AddToRoleAsync(user.Id, "Customer");
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: true);
                        var image = new CustImage
                        {
                            Path = model.photo_200_orig
                        };
                        
                        var cust = new Customer
                        {
                            Avatar = image,
                            Birthday = DateTime.Parse(model.bdate),
                            City = model.city.title,
                            Email = model.email,
                            EmailNotice = true,
                            Fio = model.first_name+" "+model.last_name,
                            NickName = model.nickname,
                            PhoneNotice = true,
                            SmsNotice = true,
                            Rate = 0,
                            User = user.Id,
                            Vk = "http://vk.com/"+model.domain
                        };
                        switch (model.sex)
                        {
                            case "1":
                                cust.ShowData = "01111111111F"; break;
                            case "2":
                                cust.ShowData = "01111111111M";break;
                            default:
                                cust.ShowData = "01111111111N";break;
                        }
                        _customer.UpdateCustomer(cust);
                        return RedirectToAction("Index","Customer");
                    }
                
            }
            AddErrorsFormResult(result);
            return View(model);
        }
        
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthManager.GetExternalLoginInfoAsync();
            if (loginInfo==null)
            {
                return RedirectToAction("Login");
            }

            var user = await UserMeneger.FindAsync(loginInfo.Login);
            if (user != null)
            {
                if (!user.EmailConfirmed) await SendEmailConfirmationAsync(user);
                if(loginInfo.ExternalIdentity.AuthenticationType.Equals("facebook", StringComparison.CurrentCultureIgnoreCase)) await StoreFacebookAuthToken(user);
                await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: true);
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                var token = loginInfo.ExternalIdentity.Claims.First(claim => claim.Type.Equals("FacebookAccessToken")).Value;
                var model = new ExternalLoginConfirmationViewmodel
                {
                    Fio = loginInfo.ExternalIdentity.Claims.First(claim => claim.Type.Equals("urn:facebook:name")).Value
                };
                var fb = new FacebookClient(token);
                dynamic myInfo = fb.Get("me?fields=id,email,first_name,last_name,gender,locale,link,birthday,picture");
                FbUserInfo userData = JsonConvert.DeserializeObject<FbUserInfo>(myInfo.ToString());
                model.Email = userData.email;
                if (string.IsNullOrEmpty(model.Email)) return View("ExternalLoginConfirmation", model);
                user = await UserMeneger.FindByEmailAsync(model.Email);
                if (user == null) return View("ExternalLoginConfirmation", model);
                if (!user.EmailConfirmed) await SendEmailConfirmationAsync(user);
                if (loginInfo.ExternalIdentity.AuthenticationType.Equals("facebook", StringComparison.CurrentCultureIgnoreCase)) await StoreFacebookAuthToken(user);
                await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: true);
                return RedirectToAction("Index", "Customer");
            }
        }

        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewmodel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index","Customer");
            }
            if (ModelState.IsValid)
            {
                var info = await AuthManager.GetExternalLoginInfoAsync();
                if (info==null)
                {
                    return RedirectToAction("ExternalLoginFailure","Auth");
                }
                var user = new AppUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                };
                var result = await UserMeneger.CreateAsync(user);
                if (result.Succeeded)
                {
                    await UserMeneger.AddToRoleAsync(user.Id, "Customer");
                    result = await UserMeneger.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        if (!_auth.CheckCustomerExist(user.Id))
                        {
                            var cst = new Customer
                            {
                                User = user.Id,
                                Email = user.Email,
                                ShowData = "00000000000N",
                                Avatar = new CustImage(),
                                Birthday = DateTime.Parse("01.01.1980"),
                                Company = new Company(),
                                EmailNotice = true,
                                PhoneNotice = true,
                                SmsNotice = true,
                                Rate = 0,
                            };
                            _customer.UpdateCustomer(cst);
                        }
                        if (!user.EmailConfirmed) await SendEmailConfirmationAsync(user);

                        if (info.ExternalIdentity.AuthenticationType.Equals("facebook", StringComparison.CurrentCultureIgnoreCase)) await StoreFacebookAuthToken(user);
                        var claims =
                            await UserMeneger.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                        claims.AddClaims(info.ExternalIdentity.Claims);
                        await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: true);
                        return RedirectToAction("Index","Customer");
                    }
                }
                AddErrorsFormResult(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Register(CustomerViewModel model, string returnUrl="")
        {
            if (!ModelState.IsValid) return View(model);
            
            var user = await UserMeneger.FindByEmailAsync(model.Email.ToLower());
            if (user == null)
            {
                var pass = System.Web.Security.Membership.GeneratePassword(8, 3);
                user = new AppUser {Email = model.Email, UserName = model.Email, PhoneNumber = model.Phone};
                var u = await UserMeneger.CreateAsync(user, pass);
                var result = await UserMeneger.AddToRoleAsync(user.Id, "Customer");
                model.UserId = user.Id;
                if (!_auth.CheckCustomerExist(user.Id))
                {
                    var cst = new Customer
                    {
                        User = user.Id,
                        Email = user.Email,
                        ShowData = "00000000000N",
                        Avatar = new CustImage(),
                        Birthday = DateTime.Parse("01.01.1980"),
                        Company = new Company(),
                        EmailNotice = true,
                        PhoneNotice = true,
                        SmsNotice = true,
                        Rate = 0,
                    };
                    _customer.UpdateCustomer(cst);
                }
                if (result.Succeeded)
                {
                    await SendEmailConfirmationAsync(user, pass);
                }
            }
            else
            {
                var result = new[] {$"Пользователь с e-mail:{model.Email} уже зарегестрирован в нашем клубе."};
                return View("Error", result);
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                return View("Thankyou");
            }
            else
            {
               return Redirect(returnUrl);
            }
        }
        [AllowAnonymous]
        public ActionResult ConfirmEmail(string user, string token)
        {
            var result = UserMeneger.ConfirmEmail(user, token);
            return View(result);
        }

        [Authorize]
        public ActionResult LogOut()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index","Home");
        }
        [AllowAnonymous]
        public ActionResult RemmemberPassword()
        {
            return View(new ForgotPassword());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemmemberPassword(ForgotPassword model)
        {
            if(!ModelState.IsValid) return View(model);
            var user = await UserMeneger.FindByEmailAsync(model.Email);
            if (user==null)
            {
                return View(model);
            }
            var code = await UserMeneger.GeneratePasswordResetTokenAsync(user.Id);
            var body = System.IO.File.ReadAllText(Server.MapPath("/Views/Shared/ResetPass.html"));
            var a = Url.Action("ResetPassword", "Auth", new { referrer = user.Id, token = code });
            var link = $"<a href=\"http://redblackclub.ru/{a}\" target=\"_blanck\">Перейти</a>";
            body = body.Replace("{0}", link);
            await PassAuth.SendMyMailAsync(body, model.Email, "KRASNOE & Черное - восстановление пароля");
            await PassAuth.SendMyMailAsync(body, "info@id-racks.ru", "KRASNOE & Черное - восстановление пароля");
            return RedirectToAction("FogotPasswordEmail");
        }

        public ActionResult ResetPassword(string referrer, string token)
        {
            return View(new ResetPassVm
            {
                Id = referrer, Token = token
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPassVm model)
        {
            var user = await UserMeneger.FindByIdAsync(model.Id);
            if (user == null) return View(model);
            var result = await UserMeneger.ResetPasswordAsync(model.Id, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("PasswordSuccessfulChanged");
            }
            ModelState.AddModelError("","Ошибка!");
            return View(model);
        } 
        public ActionResult Thankyou()
        {
            return View();
        }
        public ActionResult FogotPasswordEmail()
        {
            return View();
        }
        public ActionResult PasswordSuccessfulChanged()
        {
            return View();
        }
        private void AddErrorsFormResult(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item);
            }
        }

        private const string XsrfKey = "XsrfId";

        private async Task StoreFacebookAuthToken(AppUser user)
        {
            var cIdentity = await AuthManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            if (cIdentity != null)
            {
                var currClaim = await UserMeneger.GetClaimsAsync(user.Id);
                var facebookAccessToken = cIdentity.FindAll("FacebookAccessToken").First();
                if (!currClaim.Any())
                {
                    await UserMeneger.AddClaimAsync(user.Id, facebookAccessToken);
                }
            }
        }

        internal class ChallengeResult:HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUrl):this(provider,redirectUrl, null)
            {
                
            }

            public ChallengeResult(string provider, string redirectUrl, string userId)
            {
                LoginProvider = provider;
                RedirectUrl = redirectUrl;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUrl { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = RedirectUrl
                };
                if (UserId !=null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties,LoginProvider);
            }
        }

        private async Task SendEmailConfirmationAsync(AppUser model, string pass="")
        {
            var body = System.IO.File.ReadAllText(Server.MapPath("/Views/Shared/register.html"));
            var confToken = await UserMeneger.GenerateEmailConfirmationTokenAsync(model.Id);
            var a = Url.Action("ConfirmEmail", "Auth", new { user = model.Id, token = confToken });
            var link = $"<a href=\"http://redblackclub.ru/{a}\" target=\"_blanck\">Подтвердить E-mail</a>";
            body = body.Replace("{0}", model.Email);
            if (!string.IsNullOrEmpty(pass))
            {
                body = body.Replace("{1}", $"<h3>ВРЕМЕННЫЙ ПАРОЛЬ: <strong>{pass}</strong></h3>"); //
            }
            else
            {
                body = body.Replace("{1}", $"<br/>" +
                                           $"<h3 style=\"color:red\">Для продолжения работы с вашим аккаунтом нужно подтвердить свой адрес электронной почты</h3>" +
                                           $"<br/>");
            }
            body = body.Replace("{2}", link);
            await PassAuth.SendMyMailAsync(body, model.Email, "KRASNOE & Черное - подтверждение e-mail адреса");
        } 
    }
}