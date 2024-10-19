using Newtonsoft.Json;
using System.Text;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;

namespace ClubSite
{
    public class ThemeEditorPublishedHandler : INotificationHandler<ContentPublishedNotification>
    {
        private IContentService _contentService;

        public ThemeEditorPublishedHandler(IContentService contentService) {
            this._contentService = contentService;
        }
        public void Handle(ContentPublishedNotification notification)
        {
            foreach (var item in notification.PublishedEntities)
            {
                if(item.ContentType.Alias == "themeEditor")
                {
                    GeneratedCssFile(item);
                }
            }
        }


        private void GeneratedCssFile(Umbraco.Cms.Core.Models.IContent content)
        {
            var propertyEditorAlias = new HashSet<string>
            {
                "Umbraco.TextBox",
                "Umbraco.ColorPicker.EyeDropper"
            };
            var style = new StringBuilder();
            style.AppendLine("/* автоматически сгенерированный файл */");
            style.AppendLine(":root{");
            foreach (var property in content.Properties)
            {
                if (propertyEditorAlias.Contains(property.PropertyType.PropertyEditorAlias))
                {
                    var propertyValue = property.GetValue();
                    switch (property.PropertyType.PropertyEditorAlias)
                    {
                        case "Umbraco.ColorPicker.EyeDropper":
                            style.AppendLine(propertyValue == null ? 
                                $"--{property.Alias}:transparent;" 
                               :$"--{property.Alias}:{property.GetValue()};");
                        break;
                        default:
                            style.AppendLine("--" + property.Alias + ":" + property.GetValue() + ";");
                        break;
                    }
                }
            }
            style.Append("}");

            var root = _contentService.GetAncestors(content.Id).First();
            var websiteBaseTheme = root.GetValue<string>("websiteBaseTheme");
            if (!string.IsNullOrEmpty(websiteBaseTheme))
            {

                var themeName = JsonConvert.DeserializeObject<ThemeSelectModel>(websiteBaseTheme)?.WebsiteBaseTheme;
                if (!string.IsNullOrEmpty(themeName))
                {
                    var filenameVariablesGenerated = SiteHelpers.MapPath($"css/theme-{themeName}/vars.css");
                    using (FileStream fileStream = new FileStream(filenameVariablesGenerated, FileMode.Create))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(style.ToString());
                        fileStream.Write(buffer, 0, buffer.Length);
                    }

                    Configuration.IncreaseCacheConfigVersion();
                }
                else
                {
                    ;// throw new Exception("Не определена тема оформления раздела сайта");
                }
            }
        }

    }
}
