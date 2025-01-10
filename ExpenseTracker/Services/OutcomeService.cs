using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services
{
    public class OutcomeService : IOutcomeService
    {
        ApplicationDBContext dBContext;
        private readonly UserManager<User> _userManager;

        public OutcomeService(ApplicationDBContext DbContext, UserManager<User> userManager)
        {
            dBContext = DbContext;
            _userManager = userManager;
        }

        public async Task DeleteOutcome(int id)
        {
            var outcome = await dBContext.Outcomes.FindAsync(id);

            if (outcome != null) 
            {
                dBContext.Outcomes.Remove(outcome);
                await dBContext.SaveChangesAsync();
            }
        }

        public async Task<OutcomePaginationDTO> GetAllOutcomes(HttpContext httpContext)
        {
            List<Outcome> AllOutcomes = new List<Outcome>();

            var userID = _userManager.GetUserId(httpContext.User);
            var user = await _userManager.GetUserAsync(httpContext.User);

            AllOutcomes = await dBContext.Outcomes.Where(i => i.UserId == userID).ToListAsync();

            decimal outcomeSum = AllOutcomes.Sum(i => i.OutcomeAmount);

            return new OutcomePaginationDTO
            {
                OutcomeSum = outcomeSum,
                Balance = user.Balance,
                Outcomes = AllOutcomes
            };
        }

        public async Task<OutcomePaginationDTO> GetPaginatedOutcomes(HttpContext httpContext)
        {
            List<Outcome> AllOutcomes = new List<Outcome>();
            var pageNumberString = httpContext.Request.Query["pageNumber"].FirstOrDefault();
            int pageNumber = string.IsNullOrEmpty(pageNumberString) ? 1 : int.Parse(pageNumberString);
            int pageSize = 5;

            var userID = _userManager.GetUserId(httpContext.User);
            var user = await _userManager.GetUserAsync(httpContext.User);

            IQueryable<Outcome> query = dBContext.Outcomes.Where(i => i.UserId == userID);

            int totalIncomes = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalIncomes / (double)pageSize);
            List<Outcome> pagedOutcomes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            decimal outcomeSum = pagedOutcomes.Sum(i => i.OutcomeAmount);

            return new OutcomePaginationDTO
            {
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                Outcomes = pagedOutcomes,
                PageSize = pageSize,
                Balance = user.Balance,
                OutcomeSum = outcomeSum

            };
        }

        public async Task<Outcome> NewOutcome(HttpContext httpContext, Outcome outcomeModel)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);

            outcomeModel.User.Name = user.Name;
            outcomeModel.User = user;
            outcomeModel.UserId = user.Id;
            outcomeModel.CreatedAt = DateTime.Now;

            dBContext.Outcomes.Add(outcomeModel);
            await dBContext.SaveChangesAsync();

            return outcomeModel;
        }

        public async Task<bool> EditOutcome(HttpContext httpContext, Outcome outcomeModel, int id)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);

            var outcome = await dBContext.Outcomes.FirstOrDefaultAsync(i => i.Id == id && i.UserId == user.Id);

            if (user == null)
            {
                return false;
            }

            outcome.OutcomeAmount = outcomeModel.OutcomeAmount;
            outcome.Description = outcomeModel.Description;
            outcome.Source = outcomeModel.Source;
            outcome.UserId = user.Id;
            outcome.User.Name = user.Name;

            await dBContext.SaveChangesAsync();

            return true;
        }
    }
}
