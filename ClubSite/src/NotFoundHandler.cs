using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace ClubSite
{
    public class MultipleSite404ContentFinder : IContentLastChanceFinder
    {
        private readonly IDomainService _domainService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public MultipleSite404ContentFinder(IDomainService domainService, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _domainService = domainService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public Task<bool> TryFindContent(IPublishedRequestBuilder contentRequest)
        {
            // Сначала поищем корневой узел, который соответствует домену
            var allDomains = _domainService.GetAll(true).ToList() ?? new List<Umbraco.Cms.Core.Models.IDomain>();
            var domain = allDomains?
                .FirstOrDefault(f => f.DomainName == contentRequest.Uri.Authority
                                     || f.DomainName == $"https://{contentRequest.Uri.Authority}"
                                     || f.DomainName == $"http://{contentRequest.Uri.Authority}");

            var siteId = domain?.RootContentId;

            if (!_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                return Task.FromResult(false);
            }

            if (umbracoContext.Content == null)
                return new Task<bool>(() => contentRequest.PublishedContent is not null);


            IPublishedContent? siteRoot = null;
            if (siteId == null)
            {
                // по домену не нашли. Тогда переберем корневые узлы и попробуем найти соответствие с их адресами
                var rootNodes = umbracoContext.Content.GetAtRoot()?.Where(x => x.TemplateId > 0);
                if (rootNodes != null)
                {
                    IPublishedContent? bestNode = null;
                    var bestLen = 0;
                    var lookupUrl = contentRequest.Uri.ToString();
                    foreach (var rootNode in rootNodes)
                    {
                        var url = rootNode.Url(mode: UrlMode.Absolute);
                        // нас устроит вариант с наибольшей длиной адреса
                        if (lookupUrl.StartsWith(url) && url.Length > bestLen)
                        {
                            bestLen = url.Length;
                            bestNode = rootNode;
                        }
                    }
                    siteRoot = bestNode;
                }
            }
            else siteRoot = umbracoContext.Content.GetById(false, siteId ?? -1);

            if (siteRoot is null)
            {
                return Task.FromResult(false);
            }

            // Assuming the 404 page is in the root of the language site with alias fourOhFourPageAlias
            var notFoundNode = siteRoot.Children?.FirstOrDefault(f => f.Name == "404" && f.ContentType.Alias == "page404");

            if (notFoundNode is not null)
            {
                contentRequest.SetPublishedContent(notFoundNode);
            }

            // Return true or false depending on whether our custom 404 page was found
            return Task.FromResult(contentRequest.PublishedContent is not null);
        }
    }
}