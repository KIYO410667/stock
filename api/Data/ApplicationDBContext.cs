using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {
            
        }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfolio>().HasKey(p => new {p.StockId, p.AppUserId});
            builder.Entity<Portfolio>().HasOne(p => p.AppUser).WithMany(x => x.Portfolios).HasForeignKey(a => a.AppUserId);
            builder.Entity<Portfolio>().HasOne(p => p.Stock).WithMany(x => x.Portfolios).HasForeignKey(a => a.StockId);
            builder.Entity<Comment>().HasOne(a => a.AppUser).WithMany(c => c.Comments).HasForeignKey(a => a.AppUserId);


            builder.Entity<IdentityRole>().HasData(
                new IdentityRole{
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole{
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
        }
    }
}