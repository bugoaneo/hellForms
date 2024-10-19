namespace ClubSite.src.Models
{
    public class SearchProductReq
    {
        public int? ParentId { get; set; }
        public int CurrentCount { get; set; }
        public int PageSize { get; set; }

        public string? Title { get; set; }
    }
}
