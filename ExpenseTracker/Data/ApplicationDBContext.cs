
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Data
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Outcome> Outcomes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var user = new IdentityRole("user");
            user.NormalizedName = "user";
            

            builder.Entity<IdentityRole>().HasData(user);

            builder.Entity<User>().
             HasMany(u => u.Incomes).
             WithOne(i => i.User).
             HasForeignKey(i => i.UserId);
            builder.Entity<User>().
                HasMany(u => u.Outcomes).
                WithOne(o => o.User).
                HasForeignKey(o => o.UserId);
            builder.Entity<User>().
                Property(u => u.Balance).HasDefaultValue(0);
            builder.Entity<User>().
                Property(u => u.AllowedMinus).HasDefaultValue(0); 


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

    }
}
