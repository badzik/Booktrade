using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Booktrade
{


    public class AppUserPrincipal : ClaimsPrincipal
    {
        public AppUserPrincipal(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public string Username
        {
            get
            {
                return this.FindFirst(ClaimTypes.Name).Value;
            }
        }

        public string Name
        {
            get
            {
                return this.FindFirst("MyName").Value;
            }
        }

        public string Surname
        {
            get
            {
                return this.FindFirst(ClaimTypes.Surname).Value;
            }
        }

        public string Address
        {
            get
            {
                return this.FindFirst(ClaimTypes.StreetAddress).Value;
            }
        }

        public string PostalCode
        {
            get
            {
                return this.FindFirst("City").Value;
            }
        }

        public string Province
        {
            get
            {
                return this.FindFirst(ClaimTypes.StateOrProvince).Value;
            }
        }
        public string City
        {
            get
            {
                return this.FindFirst(ClaimTypes.Locality).Value;
            }
        }
        public string BankNumber
        {
            get
            {
                return this.FindFirst("BankNumber").Value;
            }
        }
    }
}