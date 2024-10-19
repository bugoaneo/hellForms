using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Website.Controllers;

namespace ClubSite
{
    public class CustomFormController : SurfaceController
    {
        UmbracoHelper Umbraco { get; set; }
        ILogger<CustomFormController> Logger { get; set; }
        public CustomFormController(ILogger<CustomFormController> logger, UmbracoHelper umbracoHelper, IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            Umbraco = umbracoHelper;
            Logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitCustomForm(IFormCollection data)
        {
            FormAjaxPostResult? response = null;
            var formFiles = new List<IFormFile>();

            //ModelState.AddModelError("name", "Тестовая ошибка для имени");
            //ModelState.AddModelError("", "Общая ошибка");


            try
            {
                // валидация для начала
                var pageId = int.Parse(((string?)data["pageId"]) ?? string.Empty);
                var pageNode = UmbracoContext.Content?.GetById(pageId) as IPublishedContent ?? throw new Exception("Не удалось определить страницу отправки.");
                var pageCulture = pageNode.GetCultureFromDomains();
                if (pageCulture != null)
                {
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(pageCulture);
                }

                var formId = int.Parse(((string?)data["formId"]) ?? string.Empty);
                var formNode = UmbracoContext.Content?.GetById(formId) as FormTemplate ?? throw new Exception("Не удалось определить тип формы. Попробуйте перезагрузить страницу");

                // капчу, если есть, проверяем раньше остальных
                var captcha = formNode.CustomForm
                .Where(item => ((NcSimpleFormProperty)item.Content).PropertyAlias == "captcha")
                .FirstOrDefault();
                if (captcha != null)
                {                  

                        ValidateCaptcha(data["g-recaptcha-response"]);
                    
                }

                if (ModelState.IsValid) // если капча не прошла, не проверяем остальное
                {
                    if (formNode.CustomForm != null)
                    {
                        foreach (var property in formNode.CustomForm)
                        {
                            var propertyTyped = property.Content as NcSimpleFormProperty;

                            if (propertyTyped != null)
                            {
                                string? propertyValue = string.Empty;
                                if (propertyTyped.PropertyAlias != null)
                                    propertyValue = (string?)data[propertyTyped.PropertyAlias];
                                switch (propertyTyped.PropertyType)
                                {
                                    case "Строка текста":
                                        ValidateString(propertyTyped, propertyValue); break;
                                    case "Телефон":
                                        ValidateString(propertyTyped, propertyValue); break;
                                    case "Email":
                                        ValidateString(propertyTyped, propertyValue); break;
                                    case "Дата":
                                        ValidateString(propertyTyped, propertyValue); break;
                                    case "Многострочный текст":
                                        ValidateString(propertyTyped, propertyValue); break;
                                    case "Флажок":
                                        ValidateString(propertyTyped, propertyValue); break;
                                    case "Капча":
                                        /*ValidateCaptcha(property, data[property.PropertyAlias]);*/
                                        break;
                                    case "Вложение":
                                        if (data.Files.Count() > 0)
                                        {
                                            foreach (var file in data.Files)
                                            {
                                                formFiles.Add(file);
                                            }
                                        }
                                        break;
                                    case "-- Разделитель --": break;
                                    default: ValidateUnknown(propertyTyped, propertyValue); break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException("reCAPTCHA Invalid");
                }
                if (ModelState.IsValid)
                {
                    var formContents = new Dictionary<string, string>();
                    foreach (string i in data.Keys)
                    {
                        formContents.Add(i, (string?)data[i] ?? string.Empty);
                    }

                    formContents.Remove("g-recaptcha-response"); // ни к чему хранить ответ по капче


                    // отправка почты
                    if (Configuration.Settings.SendFeedbackFormsByEmail)
                    {
                        //SendMessage(pageNode, formNode, formContents, formFiles);
                    }

                    switch (data["formTag"])
                    {
                        case "robokassa": response = RobokassaCustomFormHandler(data); break;
                        default:
                            {
                                var successMsg = formNode.SuccessMessage?.ToString();
                                if (string.IsNullOrEmpty(successMsg)) { successMsg = Umbraco.GetDictionaryValue("feedbackForm_Success"); }
                                response = FormAjaxPostResult.Success(successMsg);
                            }
                            break;
                    }

                }
                else
                    response = FormAjaxPostResult.Fail(Umbraco.GetDictionaryValue("feedbackForm_ValidationErrors"), ModelState);

            }
            catch (Exception ex)
            {
                response = FormAjaxPostResult.Fail(ex.Message);
            }
            return Json(response);
        }

        private void ValidateUnknown(NcSimpleFormProperty propertyTyped, string? propertyValue)
        {
            ModelState.AddModelError("", "Неизвестное свойство формы. Проверьте правильность полей в административной панели");
        }

        private void ValidateCaptcha(IPublishedElement captcha, StringValues stringValues)
        {
            throw new NotImplementedException();
        }

        private void ValidateString(NcSimpleFormProperty property, string? data)
        {
            if (property.PropertyRequired && string.IsNullOrEmpty(data?.Trim()))
            {
                ModelState.AddModelError(property.PropertyAlias, SiteHelpers.GetFormFieldErrorText(Umbraco, property.ErrorDescription));
            }
            else if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(property.PropertyMask))
            {
                var regex = new System.Text.RegularExpressions.Regex(property.PropertyMask);
                if (!regex.IsMatch(data))
                {
                    ModelState.AddModelError(property.PropertyAlias, SiteHelpers.GetFormFieldErrorText(Umbraco, property.ErrorDescription));
                }
            }
        }

        private void ValidateCaptcha(string data)
        {
            var capchaResponse = data;
            if (string.IsNullOrEmpty(capchaResponse))
            {
                ModelState.AddModelError("", Umbraco.GetDictionaryValue("feedbackForm_captcha-NotARobot"));
                return;
            }

            try
            {
                using (WebClient client = new WebClient())
                {

                    byte[] resp = client.UploadValues("https://www.google.com/recaptcha/api/siteverify", new NameValueCollection()
                    {
                        { "secret", Configuration.Settings.CaptchaSecret },
                        { "response", capchaResponse }
                    });

                    var recapchaResp = Newtonsoft.Json.JsonConvert.DeserializeObject<RecapchaResp>(System.Text.Encoding.UTF8.GetString(resp));
                    if (!recapchaResp.Success)
                    {
                        ModelState.AddModelError("", Umbraco.GetDictionaryValue("feedbackForm_captcha-NotARobot"));
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.LogError("Ошибка при обработке капчи", exc);
                ModelState.AddModelError("", Umbraco.GetDictionaryValue("feedbackForm_captcha-NotARobot"));
            }
        }

        private bool SendMessage(IPublishedContent pageNode, IPublishedContent formNode, Dictionary<string, string> formContents, List<IFormFile>? files = null)
        {
            var mailBodyTemplate = formNode.Value<string>("mailContents");
            var bodyHtml = true;

            if (string.IsNullOrEmpty(mailBodyTemplate))
            {
                var templateKey = formNode.HasValue("templateKey") ? "feedbackForm_TemplateFull" : formNode.Value<string>("templateKey");
                if (templateKey != null)
                {
                    mailBodyTemplate = Umbraco.GetDictionaryValue(templateKey);
                    bodyHtml = false;
                }
            }
            if (string.IsNullOrEmpty(mailBodyTemplate)) mailBodyTemplate = string.Empty;


            string[] subscribers = null;
            foreach (var block in pageNode.Value<Umbraco.Cms.Core.Models.Blocks.BlockListModel>("contentBlocks"))
            {
                if (block.Content.HasValue("subscribers"))
                {
                    subscribers = block.Content.Value<string>("subscribers")?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            if (subscribers == null)
                subscribers = pageNode.Value<string>("subscribers", fallback: Fallback.ToAncestors)?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (subscribers != null && subscribers.Length > 0)
            {
                var to = new Dictionary<string, string>();
                foreach (var subs in subscribers)
                {
                    to.Add(subs, subs);
                }


                SiteHelpers.SendMailMessage(
                    SiteHelpers.PrepareMailMessage(
                        to,
                        pageNode.Name + " -- " + formNode.Name,
                        System.Text.RegularExpressions.Regex.Replace(mailBodyTemplate.NamedFormat(formContents), "{\\w+}", string.Empty),
                        bodyHtml,
                        files
                    )
                );
            }

            return true;
        }

        private FormAjaxPostResult RobokassaCustomFormHandler(IFormCollection data)
        {
            //var desc = SiteHelpers.Or(data["Desc"], string.Empty);
            //var purchaseUrl = SiteHelpers.ConstructRobokassaPurchaseRef(data["OutSum"], desc, ConfigHelper.GetString("RobokassaPwd1"),
            // ConfigHelper.GetString("RobokassaMerchant"));
            //response = FormAjaxPostResult.Success(Umbraco.GetDictionaryValue("feedbackForm_Success"), redirectTo: purchaseUrl);
            return new FormAjaxPostResult();
        }

        public class RecapchaResp
        {
            public bool Success { get; set; }

            [JsonProperty("challenge_ts")]
            public string ChallengeTimeStamp { get; set; }

            public string Hostname { get; set; }

            [JsonProperty("error-codes")]
            public string[] ErrorCodes { get; set; }
        }
    }

}
