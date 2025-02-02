
using System.Reflection.Emit;
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
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Source> Sources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var user = new IdentityRole("user");
            user.NormalizedName = "user";
            

            builder.Entity<IdentityRole>().HasData(user);

            builder.Entity<User>().
                HasOne(u => u.Account).
                WithOne(a => a.User).
                HasForeignKey<Account>(a => a.UserId);
            builder.Entity<Account>()
                 .HasMany(a => a.Incomes)
                 .WithOne(i => i.Account)
                .HasForeignKey(i => i.AccountId);
            builder.Entity<Account>()
                .HasMany(a => a.Expenses) 
                .WithOne(e => e.Account)  
                .HasForeignKey(e => e.AccountId);
            builder.Entity<Account>().
                Property(a => a.Balance).HasDefaultValue(0);
            builder.Entity<Account>().
                Property(a => a.AllowedMinus).HasDefaultValue(0);

            builder.Entity<Expense>().ToTable("Expenses", t => t.HasTrigger("trg_UpdateBalanceOnExpenseInsert"));
            builder.Entity<Expense>().ToTable("Expenses", t => t.HasTrigger("trg_UpdateBalanceOnExpenseDelete"));
            builder.Entity<Expense>().ToTable("Expenses", t => t.HasTrigger("trg_UpdateBalanceOnExpenseEdit"));
            builder.Entity<Income>().ToTable("Incomes", t => t.HasTrigger("trg_UpdateBalanceAfterIncomeInsert"));
            builder.Entity<Income>().ToTable("Incomes", t => t.HasTrigger("trg_UpdateBalanceAfterIncomeDelete"));
            builder.Entity<Income>().ToTable("Incomes", t => t.HasTrigger("trg_UpdateBalanceOnIncomeEdit"));
            builder.Entity<Account>().ToTable("Account", t => t.HasTrigger("trg_AfterUserInsert"));


        }
        


    }
}
