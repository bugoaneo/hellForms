using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.BackOffice.Controllers;
using System.IO;

namespace ClubSite
{
    public class ClubSiteApiController : UmbracoAuthorizedApiController
    {
        //https://<url>/umbraco/backoffice/Api/ClubSiteApi/UpdateCssCache
        public IActionResult UpdateCssCache()
        {
            var newValue = Configuration.IncreaseCacheConfigVersion();
            return string.IsNullOrEmpty(newValue) ? NotFound() : Ok(newValue);
        }
    }
}
