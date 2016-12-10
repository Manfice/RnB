using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Web.Models;

namespace Web.Infrastructure
{
    public class AppSignInManager:SignInManager<AppUser, string>
    {
        public AppSignInManager(AppUserManager userManager, IAuthenticationManager authenticationManager):base(userManager, authenticationManager)
        {
            
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppUser user)
        {
            return user.GenerateUserIdentityAsync((AppUserManager)UserManager);
        }

        public static AppSignInManager Create(IdentityFactoryOptions<AppSignInManager> options, IOwinContext context)
        {
            return new AppSignInManager(context.GetUserManager<AppUserManager>(),context.Authentication);
        }
    }
}