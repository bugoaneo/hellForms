using Serilog;
using Umbraco.Cms.Core.Configuration.Models;

namespace ClubSite
{
    public class Configuration
    {
        private const string ClubSiteSection = "ClubSite";
        private const string SmtpSection = "Umbraco:CMS:Global:Smtp";
        private static readonly Model Instance = new();
        private static SmtpSettings SmtpSettings = new();

        public static Model Settings { get { return Instance; } }

        internal static void Initialize(IConfiguration config)
        {
            config.GetSection(ClubSiteSection).Bind(Instance);
            config.GetSection(SmtpSection).Bind(SmtpSettings);
        }

        public static string GetApiBaseUrl()
        {
            return Instance.ApiBaseUrl;
        }

        public static string GetClubId()
        {
            return Instance.ClubId;
        }

        public static SmtpSettings GetSmtpSettings()
        {
            return SmtpSettings;
        }        
        
        public static string? GetScheduleWidgetId()
        {
            return Instance.ScheduleWidgetId;
        }

        public static string GetWidgetBaseUrl()
        {
            return Instance.WidgetBaseUrl;
        }


        internal static string IncreaseCacheConfigVersion()
        {
            string newValue = string.Empty;
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                string json = File.ReadAllText(filePath);
                dynamic? jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                var sectionPathKey = "Umbraco:CMS:RuntimeMinification";
                if (jsonObj != null)
                {
                    var configSection = GetValueRecursively<dynamic>(sectionPathKey, jsonObj);
                    if (configSection != null)
                    {
                        var today = DateTime.Now.ToString("yyyyMMdd");
                        var curValue = configSection["Version"]?.Value as string;

                        if (curValue != null && curValue.Length == 10) {
                            var datePart = curValue[..8];
                            int.TryParse(curValue.AsSpan(8), out int generationPart);
                            if (today == datePart)
                            {
                                var generationAsStr = (generationPart + 1).ToString();
                                if (generationAsStr.Length == 1) generationAsStr = "0" + generationAsStr;
                                newValue = today + generationAsStr;
                            }
                        }
                        if (newValue.Length == 0)
                            newValue = today + "00";

                        configSection["Version"] = newValue;

                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(filePath, output);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Ошибка обновления конфигурационного файла при сбросе кэша");
            }
            return newValue;
        }

        private static T? GetValueRecursively<T>(string sectionPathKey, dynamic jsonObj)
        {
            // split the string at the first ':' character
            if (jsonObj == null) return default(T);
            var remainingSections = sectionPathKey.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the process, moving down the tree
                var nextSection = remainingSections[1];
                return GetValueRecursively<T>(nextSection, jsonObj[currentSection]);
            }
            else
            {
                // we've got to the end of the tree, set the value
                return jsonObj[currentSection];
            }
        }


        public class Model
        {
            public string ApiBaseUrl { get; set; } = string.Empty;
            public string CaptchaSecret { get; set; } = string.Empty;
            public string ClubId { get; set; } = string.Empty;
            public string ScheduleWidgetId { get; set; } = string.Empty;
            public bool SendFeedbackFormsByEmail { get; set; } = true;
            public string WidgetBaseUrl { get; set; } = string.Empty;
        }
    }
}
