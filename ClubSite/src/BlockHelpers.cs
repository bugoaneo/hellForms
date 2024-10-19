using Microsoft.AspNetCore.Html;
using NUglify.Helpers;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common;
using static ClubSite.Configuration;

namespace ClubSite
{
    public class BlockHelpers
    {
        private static Dictionary<string, string> AccentOptions = new Dictionary<string, string> { { "", "" }, { "Без акцента", "" }, { "Акцент", "_accent" }, { "Полуакцент", "_halfaccent" } };
        public static string GetBlockOuterCssClass(string baseClass, IPublishedElement? settingsModel)
        {
            var result = new List<string>();
            result.Add(baseClass);

            if (settingsModel != null)
            {

                var accent = settingsModel.Value<string>("appearance");
                if (!string.IsNullOrEmpty(accent))
                    result.Add(AccentOptions[accent]);

                if (settingsModel.Value<bool>("alternateColor"))
                    result.Add("_altColor");
                if (settingsModel.Value<bool>("centerTitle"))
                    result.Add("_centerTitle");
                if (settingsModel.Value<bool>("noMarginTop"))
                    result.Add("_noMarginTop");
                if (settingsModel.Value<bool>("noMarginBottom"))
                    result.Add("_noMarginBottom");
                if (settingsModel.Value<bool>("fullHeight"))
                    result.Add("_fullHeight");
                if (settingsModel.Value<bool>("verticalCenter"))
                    result.Add("_verticalCenter");
                if (settingsModel.Value<bool>("isFloat"))
                    result.Add("mfp-hide");
                if (settingsModel.Value<bool>("hideFromDisplay"))
                    result.Add("_hideFromDisplay");
                //if (ImageAlign == "bottom")
                //    result.Add("_image-bottom");

                if (settingsModel.HasValue("extraCssClass"))
                    result.Add(settingsModel.Value<string>("extraCssClass") ?? string.Empty);
            }
            return string.Join(" ", result);
        }

        public static string GetContainerCssClass(string baseClass, IPublishedElement? settingsModel)
        {
            var settings = new List<string>();
            settings.Add(baseClass);
            if (settingsModel != null)
            {
                if (settingsModel.Value<bool>("fullWidth"))
                    settings.Add("_full");
                if (settingsModel.Value<bool>("moreSpace"))
                    settings.Add("_moreSpace");
            }
            return string.Join(" ", settings);
        }

        public static HtmlString GetBlockOuterAttributes(string baseCssClass, IPublishedElement? settingsModel, bool skipImage = false)
        {
            string result;
            var image = skipImage ? null : settingsModel?.Value<IPublishedContent>("image");
            var imageMob = skipImage ? null : settingsModel?.Value<IPublishedContent>("imageMobile");


            var veil = settingsModel.IsNotNull() && settingsModel.HasValue("veilBackground") ? settingsModel.Value<string>("veilBackground") : "0%";

            if (image != null)
            {
                var imageUrl = image.Url();
                var imageMobileUrl = imageMob?.Url();
                if (!string.IsNullOrEmpty(imageMobileUrl))
                {
                    result = string.Format("class=\"{0}\" style=\"--sectionDesk: url('{1}'); --sectionMob: url('{2}'); --veil:{3};\"",
                        GetBlockOuterCssClass(baseCssClass, settingsModel), imageUrl, imageMobileUrl, veil);
                }
                else
                {
                    result = string.Format("class=\"{0}\" style=\"background-image:url('{1}'); --veil:{2};\"", GetBlockOuterCssClass(baseCssClass, settingsModel), imageUrl, veil);
                }
            }
            else
            {
                result = string.Format("class=\"{0}\" style=\" --veil:{1};\"",
                        GetBlockOuterCssClass(baseCssClass, settingsModel), veil);
                //result = "class=\"" + GetBlockOuterCssClass(baseCssClass, settingsModel) + "\"";
            }
            return new HtmlString(result);
        }
    }
}
