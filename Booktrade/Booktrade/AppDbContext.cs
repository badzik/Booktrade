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
        public DbSet<Message> Messages { get; set; }

        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Delivery> DeliveryOptions { get; set; }
        public DbSet<ExchangeMessage> ExchangeMessages { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public AppDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new MySqlInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<ExchangeMessage>()
                .HasMany<Book>(s => s.ProposedBooks)
                .WithMany(c => c.ExchangeMessages)
                .Map(cs =>
                {
                    cs.MapLeftKey("ExchangeMessageId");
                    cs.MapRightKey("BookId");
                    cs.ToTable("ExchangeMessageBooks");
                });

            modelBuilder.Entity<Comment>().HasOptional(s => s.BuyerSideTransaction).WithOptionalPrincipal(tr => tr.FromBuyerComment);
            modelBuilder.Entity<Comment>().HasOptional(s => s.SellerSideTransaction).WithOptionalPrincipal(tr => tr.FromSellerComment);
        }
    }
}