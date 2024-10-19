using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;

namespace ClubSite
{
    public class ThemeSelectModel
    {
        /// <summary>
        /// Выбранная тема
        /// </summary>
        public string? WebsiteBaseTheme { get; set; }


        public static ThemeSelectModel? FromModel(JToken model)
        {
            var result = model.ToObject<ThemeSelectModel>();
            return result;
        }

        public static ThemeSelectModel? FromModel(IPublishedElement model)
        {
            var prop = model.GetProperty("websiteBaseTheme");
            var result = ((JObject?)prop?.GetValue())?.ToObject<ThemeSelectModel>();
            return result;
        }

        public override string ToString()
        {
            return WebsiteBaseTheme ?? string.Empty;
        }
    }
}