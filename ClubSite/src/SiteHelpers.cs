using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Configuration;
using System.Text;
using System.IO;
using Examine;
using Umbraco.Cms.Core.Models.PublishedContent;
using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.UmbracoContext;
using MessagePack;
using Smidge.Models;
using Polly;
using Smidge;
using static ClubSite.Configuration;
using MailKit.Search;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Web.Common.Controllers;
using Examine.Search;
using ClubSite;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using ClubSite.src.Models;

namespace ClubSite
{
    /// <summary>
    /// Сводное описание для SiteHelpers
    /// </summary>
    public static class SiteHelpers
    {
        public static List<WebFile>? GetThemeBundle(HttpContext context)
        {
            if (context != null)
            {
                const string key = "themeBundleFiles";
                var list = (List<WebFile>?)context.Items[key];
                if (list == null)
                {
                    context.Items[key] = list = new List<WebFile>();
                }
                return list;
            }
            return null;

        }
        public static void PushFileToBundle(HttpContext context, WebFile file)
        {
            var bundle = GetThemeBundle(context);
            if (bundle != null)
            {
                bundle.Add(file);
            }
        }

        public async static Task<HtmlString> RenderThemeBundle(HttpContext context, SmidgeHelper smidgeHelper, string pageId, bool debugMode)
        {
            var result = string.Empty;
            var bundle = SiteHelpers.GetThemeBundle(context); // TODO: не будет работать, если паршиалы с добавляемыми файлами кэшируются
            if (bundle != null)
            {
                //var pageId = Model.Id.ToString();
                var cssBundleName = "end-css-bundle-" + pageId + "-";
                var jsBundleName = "end-js-bundle-" + pageId + "-";
                var cssCnt = 0;
                var jsCnt = 0;
                var hashSourceCss = new HashSet<string>();
                var hashSourceJs = new HashSet<string>();
                foreach (var file in bundle)
                {
                    if (file is Smidge.Models.CssFile) { cssCnt++; hashSourceCss.Add(file.FilePath); }
                    else if (file is Smidge.Models.JavaScriptFile) { jsCnt++; hashSourceJs.Add(file.FilePath); }
                }
                using (var algorithm = System.Security.Cryptography.SHA256.Create())
                {
                    cssBundleName += Convert.ToHexString(algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashSourceCss.OrderBy(x => x).Aggregate("", (a, c) => a + c))));
                    jsBundleName += Convert.ToHexString(algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(hashSourceJs.OrderBy(x => x).Aggregate("", (a, c) => a + c))));
                }

                var cssBundle = smidgeHelper.CreateCssBundle(cssBundleName);
                var jsBundle = smidgeHelper.CreateJsBundle(jsBundleName);

                foreach (var file in bundle)
                {
                    if (file is Smidge.Models.CssFile) { cssBundle.RequiresCss((Smidge.Models.CssFile)file); }
                    else if (file is Smidge.Models.JavaScriptFile) { jsBundle.RequiresJs((Smidge.Models.JavaScriptFile)file); }
                }
                if (cssCnt > 0)
                {
                    var cssRes = await smidgeHelper.CssHereAsync(cssBundleName, debug: debugMode);
                    result += cssRes;
                }
                if (jsCnt > 0)
                {
                    var jsRes = await smidgeHelper.JsHereAsync(jsBundleName, debug: debugMode);
                    result += jsRes;
                }
            }
            return new HtmlString(result);
        }


        /// <summary>
        /// Получить путь внутри сайта. Для работы пути должны сохраняться внутри Startup.Configure
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MapPath(string path) =>
            Path.Combine((string)AppDomain.CurrentDomain.GetData("WebRootPath")!, path.TrimStart('~').TrimStart('/').Replace('/', '\\'));

        /// <summary>
        /// Получить адрес ресурса в папке с текущей темой. Если путь пустой, возвращает папку темы с / в конце. Если темы нет, возвращает null
        /// </summary>
        /// <param name="current">текущая страница</param>
        /// <param name="path">Локальный путь к ресурсу внутри папки с темой или пустая строка</param>
        /// <returns>Url к файлу темы</returns>
        public static string? GetThemeJsUrl(IPublishedContent current, string path)
        {
            var root = current.Root();
            var theme = ThemeSelectModel.FromModel(root);
            if (theme != null && !string.IsNullOrEmpty(theme.WebsiteBaseTheme))
            {
                return "/scripts/theme-" + theme.WebsiteBaseTheme + "/" + path.TrimStart('/');
            }
            return null;
        }

        /// <summary>
        /// Получить адрес ресурса в папке с текущей темой
        /// </summary>
        /// <param name="current">текущая страница</param>
        /// <param name="path">Локальный путь к ресурсу внутри папки с темой или пустая строка</param>
        /// <returns></returns>
        public static string? GetThemeResourceUrl(IPublishedContent current, string path)
        {
            var root = current.Root();
            var theme = ThemeSelectModel.FromModel(root);
            if (theme != null && !string.IsNullOrEmpty(theme.WebsiteBaseTheme))
            {
                return "/css/theme-" + theme.WebsiteBaseTheme + "/" + path.TrimStart('/');
            }
            return null;
        }


        /// <summary>
        /// Возвращает схему, имя хоста и, наверное, порт для переданного URL.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlDomain(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            return new Uri(url).GetLeftPart(UriPartial.Authority);
        }

        public static string? GetDefaultImagePath(IPublishedContent current, string? cropName = null)
        {

            var defaultImage = current.Root().Value<IPublishedContent>("rootDefaultImage")?.Url();

            if (defaultImage != null)
            {
                return defaultImage;
            }
            else
            {
                var file = "img/default.jpg";

                if (cropName != null)
                {
                    file = file.GetCropUrl(cropAlias: cropName, useCropDimensions: true) ?? string.Empty;
                }
                return GetThemeResourceUrl(current, file);
            }
        }


        public static HtmlString? RemoveRteParagraph(HtmlString? text)
        {
            if (text == null) return null;
            var textstr = text.ToString().Trim();
            if (textstr.StartsWith("<p>") && textstr.EndsWith("</p>"))
            {
                var newtext = textstr[3..^4];
                if (!newtext.Contains("<p>", StringComparison.CurrentCulture))
                    return new HtmlString(newtext);
            }
            return text;
        }

        /// <summary>
        /// Возвращает первый непустой из аргументов
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static string Or(this string first, string second)
        {
            return !string.IsNullOrEmpty(first) ? first : second;
        }

        /// <summary>
        /// Возвращает true если arg != null.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object arg)
        {
            return arg != null;
        }

        /// <summary>
        /// Возвращает true, если данная строка логически не пустая (!string.IsNullOrWhiteSpace для данной строки).
        /// </summary>
        /// <param name="src">Строка.</param>
        public static bool IsNotEmpty(this string src)
        {
            return !string.IsNullOrWhiteSpace(src);
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }


        /// <summary>
        /// Возвращает правильно склонение слова связанного с числительным.
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="for1">Вариант слова для числа = 1</param>
        /// <param name="for4">Вариант слова для числа = 4</param>
        /// <param name="for5">Вариант слова для числа = 5</param>
        public static string GetNumEnding(int number, string for1, string for4, string for5)
        {
            var ending = for5;
            number = number % 100;
            if (number >= 11 && number <= 19)
            {
                ending = for5;
            }
            else
            {
                var i = number % 10;
                switch (i)
                {
                    case 1:
                        ending = for1;
                        break;

                    case 2:
                    case 3:
                    case 4:
                        ending = for4;
                        break;

                    default:
                        ending = for5;
                        break;
                }
            }

            return ending;
        }

        public static string? GetFormFieldErrorText(UmbracoHelper umbraco, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return umbraco.GetDictionaryValue("validate_GenericError");
            }
            if (text.StartsWith("validate_"))
            {
                return umbraco.GetDictionaryValue(text);
            }
            return text;
        }


        // TODO: Описание паттерна для опций: https://galdin.dev/blog/you-dont-need-iconfiguration-outside-startup/
        /// <summary>
        /// Заполнить почтовое сообщение параметрами. Больше для внутреннего использования.
        /// Используйте SendMail
        /// </summary>
        /// <param name="toAddr"></param>
        /// <param name="mailSubj"></param>
        /// <param name="mailBody"></param>
        /// <param name="bodyInHtml"></param>
        /// <returns></returns>
        internal static MimeMessage PrepareMailMessage(Dictionary<string, string> toAddr,
             string mailSubj, string mailBody, bool bodyInHtml, List<IFormFile>? files = null)
        {
            var message = new MimeMessage();
            foreach (var to in toAddr.Keys)
            {
                message.To.Add(new MailboxAddress(to, toAddr[to]));
            }
            message.Subject = mailSubj;

            var multipart = new Multipart("mixed");

            var body = new TextPart(bodyInHtml ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Text)
            {
                Text = mailBody
            };

            multipart.Add(body);

            if (files != null && files.Count() > 0)
            {
                foreach (var file in files)
                {
                    var attachment = new MimePart(file.ContentType)
                    {
                        Content = new MimeContent(file.OpenReadStream()),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = file.Name
                    };

                    multipart.Add(attachment);
                }
            }

            message.Body = multipart;


            return message;
        }

        /// <summary>
        /// Отправить подготовленный email. Параметры отправки из SMTP web.config'а, если отправитель не заполнен, тоже берется оттуда.
        /// Для внутреннего использования. Для внешнего лучше использовать SendMail
        /// </summary>
        /// <param name="message"></param>
        internal static void SendMailMessage(MimeMessage message)
        {
            var smtpSettings = Configuration.GetSmtpSettings();
            if (smtpSettings != null)
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    if (message.From.Count == 0)
                    {
                        message.From.Add(new MailboxAddress(smtpSettings.From, smtpSettings.From));
                    }
                    client.Connect(smtpSettings.Host, smtpSettings.Port, (SecureSocketOptions)smtpSettings.SecureSocketOptions);
                    client.Authenticate(smtpSettings.Username, smtpSettings.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
        }

        public static void SendMail(Dictionary<string, string> toAddr,
             string mailSubj, string mailBody, bool bodyInHtml = true)
        {
            var message = PrepareMailMessage(toAddr, mailSubj, mailBody, bodyInHtml);
            SendMailMessage(message);
        }

        /// <summary>
        /// Возвращает строку, где вхождения вида {name} заменены на значения из соответствующих свойств объекта source.
        /// </summary>
        /// <param name="src">Исходная строка (шаблон).</param>
        /// <param name="data">Объект-источних значений.</param>
        //public static string NamedFormat(this string src, object data)
        //{
        //    return src.NamedFormat(data.ToStringDictionary());
        //}

        /// <summary>
        /// Возвращает строку, где вхождения вида {name} заменены на значения из соответствующих свойств объекта source.
        /// </summary>
        /// <param name="src">Исходная строка (шаблон).</param>
        /// <param name="data">Объект-источних значений.</param>
        public static string NamedFormat(this string src, IDictionary<string, string> data)
        {
            // Если шаблон пустой, то на выходе получаем пустую строку.
            //if (src.IsEmpty()) return string.Empty;

            // Если в данных ничего нет, то на выходе получаем исходную строку.
            //if (data.IsNull() || data.Any().Not()) return src;

            var result = new StringBuilder(src.Length * 2);

            using (var reader = new StringReader(src))
            {
                var expression = new StringBuilder();
                int ch = -1;

                var state = State.OutsideExpression;
                do
                {
                    switch (state)
                    {
                        case State.OutsideExpression:
                            ch = reader.Read();
                            switch (ch)
                            {
                                case -1:
                                    state = State.End;
                                    break;
                                case '{':
                                    state = State.OnOpenBracket;
                                    break;
                                case '}':
                                    result.Append('}');
                                    break;
                                default:
                                    result.Append((char)ch);
                                    break;
                            }
                            break;

                        case State.OnOpenBracket:
                            ch = reader.Read();
                            switch (ch)
                            {
                                case -1:
                                    state = State.End;
                                    break;
                                case '{':
                                    result.Append('{');
                                    state = State.OutsideExpression;
                                    break;
                                case '}':
                                    result.Append("{}");
                                    state = State.OutsideExpression;
                                    break;
                                default:
                                    expression.Append((char)ch);
                                    state = State.InsideExpression;
                                    break;
                            }
                            break;

                        case State.InsideExpression:
                            ch = reader.Read();
                            switch (ch)
                            {
                                case -1:
                                    result.Append('{');
                                    result.Append(expression);
                                    state = State.End;
                                    break;
                                case '{':
                                    result.Append('{');
                                    break;
                                case '}':
                                    result.Append(OutExpression(data, expression.ToString()));
                                    expression.Length = 0;
                                    state = State.OutsideExpression;
                                    break;
                                default:
                                    expression.Append((char)ch);
                                    break;
                            }
                            break;

                        case State.OnCloseBracket:
                            ch = reader.Read();
                            switch (ch)
                            {
                                case '}':
                                    result.Append('}');
                                    state = State.OutsideExpression;
                                    break;
                                default:
                                    expression.Append((char)ch);
                                    state = State.OutsideExpression;
                                    break;
                            }
                            break;

                        default:
                            throw new InvalidOperationException("Invalid state.");
                    }
                } while (state != State.End);
            }

            return result.ToString();
        }


        private static string OutExpression(IDictionary<string, string> data, string expression)
        {
            expression = expression.Trim();

            if (string.IsNullOrEmpty(expression))
                return string.Empty;

            if (!data.ContainsKey(expression))
                return $"{{{expression}}}";

            return data[expression] ?? string.Empty;
        }

        private enum State
        {
            OutsideExpression,
            OnOpenBracket,
            InsideExpression,
            OnCloseBracket,
            End
        }

        public static string GetTitle(IPublishedContent node, DateTime dateTime, string titlePage, string nameCityField, string nameDateCheckBox)
        {
            var title = "";
            if (node.Value<bool>(nameDateCheckBox) == true)
            {
                title += dateTime.ToString("dd.MM.yyyy");
                if (node.HasValue(nameCityField))
                {
                    title += " " + node.Value<string>(nameCityField) + ". " + titlePage;
                }
                else
                {
                    title += " " + titlePage;
                }
            }
            else
            {
                if (node.HasValue(nameCityField))
                {
                    title += node.Value<string>(nameCityField) + ". " + titlePage;
                }
                else
                {
                    title += titlePage;
                }
            }
            return title;
        }

        public static string ProductPriceNamedFormat(decimal price, decimal amountDiscount, string? typeDiscount)
        {
            if (price == 0)
                return $"<div>{price.ToString("0,0")} руб</div>";
            else
                if (amountDiscount == 0)
                return $"<div>{price.ToString("0,0")} руб</div>";
            else
                switch (typeDiscount)
                {
                    case "Процент от цены":
                        return $"<s>{price.ToString("0,0")} руб</s><div >{((double)price - System.Math.Round((double)(price * amountDiscount / 100))).ToString("0,0")} руб</div>";
                    case "Фиксированная сумма от цены":
                    default:
                        return $"<s>{price.ToString("0,0")} руб</s><div >{(price - amountDiscount).ToString("0,0")} руб</div>";
                }
        }

        public static SearchResp SearchProduct(
           IExamineManager _examineManager,
           UmbracoHelper _umbracoHelper,
           SearchProductReq req)
        {
            IEnumerable<ISearchResult> searchResults = Array.Empty<ISearchResult>();
            SearchResp response = new SearchResp();

            if (_examineManager.TryGetIndex("ExternalIndex", out IIndex? index))
            {
                var parentId = req.ParentId;
                if (parentId == null)
                    parentId = _umbracoHelper.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias == "homePage")?.FirstChildOfType("catalog")?.Id;

                //var nodeIdToCheck = "," + parentId.ToString() + ",";
                IBooleanOperation q = index.Searcher
                    .CreateQuery("content")
                    .NodeTypeAlias("product");

				if (req.ParentId != null)
					q.And().RangeQuery<int>(new string[] { "parentID" }, req.ParentId, req.ParentId);

				if (req.Title != null)
                    q.And().Field("title", req.Title);

                //searchResults = q.Execute()
                //.Where(w => w["__Path"].Contains(nodeIdToCheck));
				searchResults = q.SelectAllFields().Execute(new QueryOptions(skip: 0, take: 9999));

				response.TotalCount = searchResults.Count();
                if (searchResults.Any())
                    response.Result = _umbracoHelper.Content(searchResults.Skip(req.CurrentCount).Take(req.PageSize).Select(x => x.Id));

                response.Categorys = new List<SearchCategorysResp>();
                foreach (var product in _umbracoHelper.Content(searchResults.Select(x => x.Id)))
                {
                    if (product != null)
                    {
                        if (response.Categorys.FirstOrDefault(x => x.Content.Id == product.Parent.Id) == null)
                            response.Categorys.Add(new SearchCategorysResp
                            {
                                Content = product.Parent,
                                CountPoroduct = 1
                            });
                        else
                            response.Categorys.FirstOrDefault(x => x.Content.Id == product.Parent.Id).CountPoroduct++;
                    }
                }
            }

            return response;
        }

        public static SearchResp SearchNews(
            IExamineManager _examineManager,
            UmbracoHelper _umbracoHelper,
            SearchNewsReq req)
        {
            //IEnumerable<ISearchResult> searchResults = Array.Empty<ISearchResult>();
            SearchResp response = new SearchResp();

            if (_examineManager.TryGetIndex("ExternalIndex", out IIndex? index))
            {
                var nodeIdToCheck = "," + req.ParentId.ToString() + ",";
                var results = index.Searcher
                    .CreateQuery("content")
                    .NodeTypeAlias("newsItem").OrderByDescending(new SortableField("newsDate", SortType.Long))
                    .Execute()
                    .Where(w => w["__Path"].Contains(nodeIdToCheck));

                response.TotalCount = results.Count();
                if (results.Any())
                    response.Result = _umbracoHelper.Content(results.Skip(req.CurrentCount).Take(req.PageSize).Select(x => x.Id));
            }

            return response;
        }
        public static List<int> GetYearsList(
           IExamineManager _examineManager,
           UmbracoHelper _umbracoHelper,
           int parentId)
        {
            IEnumerable<IPublishedContent> contents = null;

            if (_examineManager.TryGetIndex("ExternalIndex", out IIndex? index))
            {
                var nodeIdToCheck = "," + parentId.ToString() + ",";
                var searchResults = index.Searcher
                    .CreateQuery("content")
                    .NodeTypeAlias("newsItem")
                    .OrderByDescending(new SortableField("newsDate", SortType.Long))
                    .Execute()
                    .Where(w => w["__Path"].Contains(nodeIdToCheck));

                if (searchResults.Any())
                    contents = _umbracoHelper.Content(searchResults.Select(x => x.Id));
            }

            if (contents == null) { return new List<int>(); }

            var allDate = contents.ToList().Select(x => x.Value<DateTime>("newsDate"));
            List<int> year = new List<int>();

            if (allDate != null)
            {
                foreach (var d in allDate)
                {
                    if (!year.Any(x => x == d.Year)) { year.Add(d.Year); }
                }
            }

            return year.ToList();
        }
        /// <summary>
        /// Получить запрошенную страницу новостей
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="page">Начинается с 1</param>
        /// <param name="pageSize"></param>
        /// <param name="_umbraco"></param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> GetPagedNews(IPublishedContent parent, int page, int pageSize, IUmbracoContext _umbraco, IExamineManager _examineManager)
        {
            if (_examineManager.TryGetIndex("ExternalIndex", out var index))
            {
                IEnumerable<ISearchResult> results;
                var searcher = index.Searcher;
                if (page > 1)
                {
                    results = searcher.CreateQuery("content")
                        .NodeTypeAlias("newsItem")
                        .And()
                        .Field("newsStorageNodeId", parent.Id)//Если убрать преобразование в строку, в какой то моент появилась ошибка Lucene о том
                                                              //что поле newsStorageNodeId текстовое. Хотя сначала принимало int 
                        .OrderByDescending(new Examine.Search.SortableField("newsDate", Examine.Search.SortType.Long))
                        .Execute().Skip(pageSize * (page - 1)).Take(pageSize); // по какой-то причине, если не делать эту -1, пропускается одна новость. Из-за этого код пришлось разделить на 2 ветки - первую страницу обрабатывать отдельно

                }
                else
                {
                    results = searcher.CreateQuery("content")
                        .NodeTypeAlias("newsItem")
                        .And()
                        .Field("newsStorageNodeId", parent.Id)//Если убрать преобразование в строку, в какой то моент появилась ошибка Lucene о том
                                                              //что поле newsStorageNodeId текстовое. Хотя сначала принимало int 
                        .OrderByDescending(new Examine.Search.SortableField("newsDate", Examine.Search.SortType.Long))
                        .Execute().Take(pageSize);

                }
                if (results.Any())
                {
                    foreach (var res in results)
                    {
                        var node = _umbraco.Content?.GetById(Convert.ToInt32(res.Id));
                        if (node != null) // на случай некорректного индекса
                            yield return node;
                    }
                }
            }
        }

        /// <summary>
        /// Ищет новости внутри заданного узла, независимо от вложенности. Возвращает заданное число самых свежих по дате новости
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="count"></param>
        /// <param name="_umbraco"></param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> GetLastNNews(IPublishedContent parent, int count, IUmbracoContext _umbraco, IExamineManager _examineManager)
        {
            if (_examineManager.TryGetIndex("ExternalIndex", out var index))
            {
                var searcher = index.Searcher;
                var nodeIdToCheck = "," + parent.Id.ToString() + ",";
                var results = searcher.CreateQuery("content")
                    .NodeTypeAlias("newsItem")
                    .OrderByDescending(new SortableField("newsDate", SortType.Long))
                    .Execute()
                    .Where(w => w["__Path"].Contains(nodeIdToCheck))
                    .Take(count);
                if (results.Any())
                {
                    foreach (var res in results)
                    {
                        var node = _umbraco.Content?.GetById(Convert.ToInt32(res.Id));
                        if (node != null)
                            yield return node;
                    }
                }
            }
        }
    }
}