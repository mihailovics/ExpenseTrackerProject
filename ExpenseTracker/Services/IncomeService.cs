using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ExpenseTracker.Services
{
    public class IncomeService : IIncomeService
    {
        ApplicationDBContext dBContext;
        private readonly UserManager<User> _userManager;

        public IncomeService(ApplicationDBContext DbContext, UserManager<User> userManager) 
        {
            dBContext = DbContext;
            _userManager = userManager;
        }

        public async Task<IncomePaginationDTO> GetPaginatedIncomes(HttpContext httpContext)
        {
            List<Income> AllIncomes = new List<Income>();
            var pageNumberString = httpContext.Request.Query["pageNumber"].FirstOrDefault();
            int pageNumber = string.IsNullOrEmpty(pageNumberString) ? 1 : int.Parse(pageNumberString);

            var pageSizeString = httpContext.Request.Query["pageSize"].FirstOrDefault();
            int pageSize = string.IsNullOrEmpty(pageSizeString) ? 5 : int.Parse(pageSizeString);

            var userID = _userManager.GetUserId(httpContext.User);
            var user = await _userManager.GetUserAsync(httpContext.User);

            IQueryable<Income> query = dBContext.Incomes.Where(i => i.UserId == userID);

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
                Balance = user.Balance,
                IncomeSum = incomeSum
                
            };
        }

        public async Task<IncomePaginationDTO> GetAllIncomes(HttpContext httpContext)
        {
            List<Income> AllIncomes = new List<Income>();

            var userID = _userManager.GetUserId(httpContext.User);
            var user = await _userManager.GetUserAsync(httpContext.User);

            AllIncomes = await dBContext.Incomes.Where(i => i.UserId == userID).ToListAsync();

            decimal incomeSum = AllIncomes.Sum(i => i.IncomeAmount);

            return new IncomePaginationDTO
            {
                IncomeSum = incomeSum,
                Balance = user.Balance,
                Incomes = AllIncomes
            };
        }

        public async Task<Income> NewIncome(HttpContext httpContext, Income incomeModel)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);

            incomeModel.User.Name = user.Name;
            incomeModel.User = user;
            incomeModel.UserId = user.Id;
            incomeModel.CreatedAt = DateTime.Now;

            dBContext.Incomes.Add(incomeModel);
            await dBContext.SaveChangesAsync();

            return incomeModel;
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
    }
}
