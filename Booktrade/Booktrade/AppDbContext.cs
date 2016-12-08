using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Booktrade.Models;

namespace Booktrade
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Delivery> DeliveryOptions { get; set; }
        public AppDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new MySqlInitializer());
        }
    }
}