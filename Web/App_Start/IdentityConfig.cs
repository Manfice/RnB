using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Facebook;
using Owin;
using Web.Infrastructure;
using Web.Models;

namespace Web
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.CreatePerOwinContext<AppSignInManager>(AppSignInManager.Create);
            app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Auth/Login"),
                ExpireTimeSpan = TimeSpan.FromMinutes(50),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUser>(
                        TimeSpan.FromDays(1), (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            
            //Facebook 

            var fbOptions = new FacebookAuthenticationOptions
            {
                AppId = "321205768264510",
                AppSecret = "33d7feb0ffb5030205e9870e356c0039",
                AuthenticationType = "Facebook",
                Caption = "Авторизация через Facebook"
            };
            fbOptions.Scope.Add("email");
            fbOptions.Scope.Add("public_profile");
            fbOptions.Provider = new FacebookAuthenticationProvider
            {
                OnAuthenticated = async context =>
                {
                    context.Identity.AddClaim(new Claim("FacebookAccessToken",context.AccessToken));
                    foreach (var claim in context.User)
                    {
                        var clameType = $"urn:facebook:{claim.Key}";
                        var claimValue = claim.Value.ToString();
                        if (!context.Identity.HasClaim(clameType, claimValue))
                        {
                            context.Identity.AddClaim(new Claim(clameType, claimValue, "XmlSchemaString", "Facebook"));
                        }
                    }
                    await Task.Delay(100);
                }
            };

            fbOptions.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(fbOptions);//appId: "321205768264510", appSecret: "33d7feb0ffb5030205e9870e356c0039"

            //Vk
            
            //Ok


        }
    }
}