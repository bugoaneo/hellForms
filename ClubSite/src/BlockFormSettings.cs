using Microsoft.AspNetCore.Html;
using NUglify.Helpers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using static ClubSite.Configuration;
using Umbraco.Cms.Core.Models.Blocks;

namespace ClubSite
{
    public class BlockFormSettings
    {
        public static string GetBlockOuterCssClass(string baseClass, IPublishedElement? settingsModel)
        {
            var result = new List<string>();
            result.Add(baseClass);
            if (settingsModel != null)
            {
                if (settingsModel.Value<bool>("isTinyContainer"))
                    result.Add("containerTiny");
                if (settingsModel.HasValue("additionalClass"))
                    result.Add(settingsModel.Value<string>("additionalClass") ?? string.Empty);
                if (settingsModel.Value<bool>("hideFromDisplay"))
                    result.Add("_hideFromDisplay");
            }
            return string.Join(" ", result);
        }
    }
}