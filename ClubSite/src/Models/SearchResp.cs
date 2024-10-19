using Umbraco.Cms.Core.Models.PublishedContent;

namespace ClubSite
{
	public class SearchResp
	{
		public IEnumerable<IPublishedContent>? Result { get; set; }
		public int TotalCount { get; set; }

		public List<SearchCategorysResp>? Categorys { get; set; }
	}

	public class SearchCategorysResp
	{
		public IPublishedContent Content { get; set; }
		public int CountPoroduct { get; set; }
	}
}
