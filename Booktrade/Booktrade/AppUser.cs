﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booktrade
{
    public class AppUser : IdentityUser
    {
        public string Country { get; set; }
    }
}