namespace SMS.WebApp.Models.Common
{
    public class PaginationState
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public int TotalCount { get; set; } = 0;
        public bool HasPreviousPage { get; set; } = false;
        public bool HasNextPage { get; set; } = false;
        public string SearchTerm { get; set; } = string.Empty;
    }
}
