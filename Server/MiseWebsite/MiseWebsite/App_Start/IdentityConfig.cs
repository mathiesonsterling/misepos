using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Mise.Core.ValueItems;
using MiseWebsite.Database.Implementation;
using MiseWebsite.Models;
using MiseWebsite.Services;
using MiseWebsite.Services.Implementation;

namespace MiseWebsite
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        private readonly IMiseAccountService _accountService;
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager,
            IMiseAccountService accountService)
            : base(userManager, authenticationManager)
        {
            _accountService = accountService;
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            var restaurantDAL = new ManagementDAL();
            var accountDAL = new AccountDAL();
            var accountService = new MiseAccountService(restaurantDAL, accountDAL);
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication, accountService);
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var baseRes = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);

            if (baseRes == SignInStatus.Success)
            {
                return baseRes;
            }

            var miseUsers = (await _accountService.GetAreasUserHasAccessTo(new EmailAddress(userName), new Password(password))).ToList();

            if (!miseUsers.Any() || miseUsers.All(mu => mu == MiseWebsiteAreas.None))
            {
                return baseRes;
            }

            var claimsIdentity = new ClaimsIdentity("TwoFactorCookie");
            claimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userName));
            AuthenticationManager.SignIn(claimsIdentity);

            var principle = new GenericPrincipal(claimsIdentity, new string[] {});
            HttpContext.Current.User = principle;
            Thread.CurrentPrincipal = principle;

            FormsAuthentication.SetAuthCookie(userName, isPersistent);
            return SignInStatus.Success;
        }
    }
}
