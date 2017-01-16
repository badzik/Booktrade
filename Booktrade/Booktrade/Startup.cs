using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

namespace Booktrade
{
    public class Startup
    {

        public static Func<UserManager<AppUser>> UserManagerFactory { get; private set; }

        public void Configuration(IAppBuilder app)
        {

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/auth/login")
            });

            // configure the user manager
            UserManagerFactory = () =>
            {
                var usermanager = new UserManager<AppUser>(
                    new UserStore<AppUser>(new AppDbContext()));
                // allow alphanumeric characters in username
                usermanager.UserValidator = new UserValidator<AppUser>(usermanager)
                {
                    AllowOnlyAlphanumericUserNames = false
                };
                usermanager.ClaimsIdentityFactory = new AppUserClaimsIdentityFactory();

                var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Booktrade");
                usermanager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<AppUser>(provider.Create("PasswordReset"));

                return usermanager;
            };

        }
    }
}