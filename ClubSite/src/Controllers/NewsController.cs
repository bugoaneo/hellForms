using Examine;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Website.Controllers;

namespace ClubSite
{
	public class NewsController : SurfaceController
	{
		UmbracoHelper Umbraco { get; set; }
		IExamineManager ExamineManager { get; set; }

		public NewsController(
			ILogger<CustomFormController> logger,
			UmbracoHelper umbracoHelper,
			IExamineManager examineManager,
			IUmbracoContextAccessor umbracoContextAccessor,
			IUmbracoDatabaseFactory databaseFactory,
			ServiceContext services,
			AppCaches appCaches,
			IProfilingLogger profilingLogger,
			IPublishedUrlProvider publishedUrlProvider)
			: base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{
			this.ExamineManager = examineManager;
			this.Umbraco = umbracoHelper;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SearchNews(SearchNewsReq req)
		{
			if (req.ParentId == 0) { return BadRequest("parentId null"); }
			var searchResult = SiteHelpers.SearchNews(
				this.ExamineManager,
				this.Umbraco,
				req);
			ViewBag.SearchResult = searchResult.Result?.ToList();
			return View("newsItem", searchResult.Result?.ToList());
		}
	}
}
