namespace ExpenseTracker.Helpers
{
    public interface ICommonMethods
    {
        Task<List<int>> GetDistinctYearsAsync(HttpContext httpContext);
        Task<List<int>> GetDistinctMonthsAsync(HttpContext httpContext);
        Task<List<string>> GetDistinctSourcesAsync(HttpContext httpContext);
    }
}
