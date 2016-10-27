using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Booktrade
{
    public class AppUserClaimsIdentityFactory : ClaimsIdentityFactory<AppUser>
    {
        public override async Task<ClaimsIdentity> CreateAsync(
            UserManager<AppUser,string> manager,
            AppUser user,
            string authenticationType)
        {
            var identity = await base.CreateAsync(manager, user, authenticationType);
            identity.AddClaim(new Claim(ClaimTypes.StreetAddress, user.Address));
            identity.AddClaim(new Claim(ClaimTypes.PostalCode, user.PostalCode));
            identity.AddClaim(new Claim(ClaimTypes.StateOrProvince, user.Province));
            //identity.AddClaim(new Claim(ClaimTypes.StreetAddress, user.City));
            //identity.AddClaim(new Claim(ClaimTypes.StreetAddress, user.BankNumber));
            identity.AddClaim(new Claim("MyName", user.Name));
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.Surname));
            return identity;
        }
    }
}