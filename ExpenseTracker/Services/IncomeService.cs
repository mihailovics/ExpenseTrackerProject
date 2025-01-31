using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ExpenseTracker.Services
{
    public class IncomeService : IIncomeService
    {
        ApplicationDBContext dBContext;
        private readonly UserManager<User> _userManager;
        private readonly ICommonMethods _commonMethods;

        public IncomeService(ApplicationDBContext DbContext, UserManager<User> userManager, ICommonMethods commonMethods) 
        {
            dBContext = DbContext;
            _userManager = userManager;
            _commonMethods = commonMethods;
        }

        public async Task<IncomePaginationDTO> GetPaginatedIncomes(HttpContext httpContext)
        {
            //List<Income> AllIncomes = new List<Income>();
            var pageNumberString = httpContext.Request.Query["pageNumber"].FirstOrDefault();
            int pageNumber = string.IsNullOrEmpty(pageNumberString) ? 1 : int.Parse(pageNumberString);

            var pageSizeString = httpContext.Request.Query["pageSize"].FirstOrDefault();
            int pageSize = string.IsNullOrEmpty(pageSizeString) ? 5 : int.Parse(pageSizeString);

            var years = httpContext.Request.Query["year"].FirstOrDefault();
            var month = httpContext.Request.Query["month"].FirstOrDefault();
            var source = httpContext.Request.Query["source"].FirstOrDefault();


            var user = await _userManager.GetUserAsync(httpContext.User);
            var account = await _commonMethods.GetAccountForUserAsync(user.Id);

            IQueryable<Income> query = dBContext.Incomes.Where(i => i.AccountId == account.Id);

            if (!string.IsNullOrEmpty(years))
            {
                var yearInt = int.Parse(years);
                query = query.Where(i => i.CreatedAt.Year == yearInt);
            }

            if (!string.IsNullOrEmpty(month))
            {
                var monthInt = int.Parse(month);
                query = query.Where(i => i.CreatedAt.Month == monthInt);
            }

            if (!string.IsNullOrEmpty(source))
            {
                query = query.Where(i => i.Source == source);
            }

            int totalIncomes = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalIncomes / (double)pageSize);
            List<Income> pagedIncomes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            decimal incomeSum = pagedIncomes.Sum(i => i.IncomeAmount);

            return new IncomePaginationDTO
            {
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                Incomes = pagedIncomes,
                PageSize = pageSize,
                Balance = account.Balance,
                IncomeSum = incomeSum
            };
        }

        public async Task<bool> NewIncome(string userId, Income incomeModel)
        {
            try
            {
                var account = await _commonMethods.GetAccountForUserAsync(userId);

                incomeModel.Account = account;
                incomeModel.AccountId = account.Id;
                incomeModel.CreatedAt = DateTime.Now;

                dBContext.Incomes.Add(incomeModel);
                await dBContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public async Task DeleteIncome(int id)
        {
            var income = await dBContext.Incomes.FindAsync(id);
            if (income != null) 
            {
                dBContext.Incomes.Remove(income);
                await dBContext.SaveChangesAsync();
            }
        }

        public async Task<bool> EditIncome(string userId, Income updatedIncome, int id)
        {
            var account = await _commonMethods.GetAccountForUserAsync(userId);

            var income = await dBContext.Incomes.FirstOrDefaultAsync(i => i.Id == id && i.AccountId == account.Id);

            if(userId == null)
            {
                return false;
            }

            income.IncomeAmount = updatedIncome.IncomeAmount;
            income.Description = updatedIncome.Description;
            income.Source = updatedIncome.Source;
            income.AccountId = account.Id;


            //income.CreatedAt = DateTime.Now;

            await dBContext.SaveChangesAsync();

            return true;

        }

        public async Task<Income> FindByid(int id)
        {
            var income = await dBContext.Incomes.FindAsync(id);
            return income;  
        }
    }
}
