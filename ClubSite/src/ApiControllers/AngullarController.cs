using System.Net;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Core.Scoping;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;
using static Umbraco.Cms.Core.Constants.HttpContext;
using static ClubSite.src.ApiControllers.AngullarController;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;

namespace ClubSite.src.ApiControllers
{
    public class AngullarController : SurfaceController
    {

        private readonly IConfiguration Configuration;
        private readonly UmbracoHelper _Umbraco;
        private readonly IContentService _contentService;
        private readonly IAppPolicyCache _runtimeCache;
        private readonly IAppCache _requestCache;
        private readonly IsolatedCaches _isolatedCaches;
        private readonly IPublishedContentQuery _contentQuery;
        private readonly IScopeProvider _scopeProvider;

        private readonly HttpStatusCode[] StatusOk = { HttpStatusCode.OK, HttpStatusCode.Created };

        public AngullarController(
            UmbracoHelper Umbraco,
            IContentService contentService,
            IConfiguration configuration,
            IPublishedContentQuery contentQuery,
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,

            IScopeProvider scopeProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _contentService = contentService;
            _Umbraco = Umbraco;
            Configuration = configuration;
            _contentQuery = contentQuery;
            _runtimeCache = appCaches.RuntimeCache;
            _requestCache = appCaches.RequestCache;
            _isolatedCaches = appCaches.IsolatedCaches;
            _scopeProvider = scopeProvider;
        }
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<ActionResult> SaveData([FromBody] JsonElement jsonData)
        {
            var data = jsonData.GetProperty("data");
            var formData = data.GetProperty("data");
            var formMedia = data.GetProperty("files");
            List<ImageObject> imageList = JsonConvert.DeserializeObject<List<ImageObject>>(formMedia.ToString());
            bool checkHeading = true;
            ReceivedData receivedData = new ReceivedData();
            ReceivedTopic receivedTopic = new ReceivedTopic();
            ReceivedItem receivedItem = new ReceivedItem();
            foreach (var item in formData.EnumerateArray())
            {
                try
                {
                    string type = item.GetProperty("type").GetString();
                    string text = item.TryGetProperty("text", out var textProp) ? textProp.GetString() : null;
                    string value = item.TryGetProperty("value", out var valueProp) ? valueProp.GetString() : null;
                    string label = item.TryGetProperty("label", out var labelProp) ? labelProp.GetString() : null;
                    if (type == "heading")
                    {
                        if (receivedItem != null) { receivedTopic.receivedTopics.Add(receivedItem); }
                        receivedItem = new ReceivedItem();
                        var tag = item.GetProperty("tag").GetString();
                        if (checkHeading)
                        {

                            topic topic = new topic()
                            {
                                tag = tag,
                                text = text
                            };
                            receivedTopic.topics.Add(topic);
                        }
                        else
                        {

                            if (receivedTopic != null)
                                receivedData.receivedTopics.Add(receivedTopic);
                            receivedTopic = new ReceivedTopic();
                            topic topic = new topic()
                            {
                                tag = tag,
                                text = text
                            };
                            receivedTopic.topics.Add(topic);
                        }
                        checkHeading = true;

                    }
                    else
                    if (type == "div_caption")
                    {
                        checkHeading = false;
                        if (receivedItem.question != null)
                        {
                            receivedTopic.receivedTopics.Add(receivedItem);
                        }
                        receivedItem = new ReceivedItem();
                        receivedItem.question = text;
                    }
                    else
                    if (type == "field")
                    {
                        if (receivedItem.question == null && !receivedItem.answer.Any())
                        {
                            receivedItem = new ReceivedItem();
                        }
                        checkHeading = false;
                        if (label != null)
                        {
                            receivedItem.answer.Add("^" + label);
                        }
                        else
                        {
                            receivedItem.answer.Add(value);
                        }

                    }
                }
                catch
                {

                }
               
            }
            var html = GenerateHtml(receivedData);
            List<IFormFile> files = new List<IFormFile>();
            (html,files) = ReplaceImagesWithBase64(html, imageList); 
            SaveData(JsonConvert.SerializeObject(receivedData));
            //SendRequest(html, files); 
            bool result = true;
            return Ok(JsonConvert.SerializeObject(result));
        }
        private void SendRequest(string message, List<IFormFile> files)
        {
            var to = new Dictionary<string, string>();
            to.Add("tolya579@list.ru", "tolya579@list.ru");
            SiteHelpers.SendMailMessage(
                SiteHelpers.PrepareMailMessage(
                    to,
                    "Заявка",
                    message,
                    true,
                    files


                )
            );
        }
        public IFormFile ConvertBase64ToIFormFile(string base64String, string fileName)
        {
            string base64Data = base64String.Split(',')[1];
            byte[] fileBytes = Convert.FromBase64String(base64Data);
            var stream = new MemoryStream(fileBytes);
            IFormFile formFile = new FormFile(stream, 0, fileBytes.Length, fileName, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf" 
            };

            return formFile;
        }

        private (string,List<IFormFile>) ReplaceImagesWithBase64(string htmlContent, List<ImageObject> mediaList)
        {
            
            var files = new List<IFormFile>();
            foreach (var mediaObject in mediaList)
            {
                if (mediaObject.FileName.EndsWith(".png") || mediaObject.FileName.EndsWith(".jpg"))
                {
                    htmlContent = htmlContent.Replace(mediaObject.FileName, $"<img src='{mediaObject.Base64Src}' alt='{mediaObject.FileName}' />");
                }
                else if (mediaObject.FileName.EndsWith(".pdf"))
                {
                    htmlContent = htmlContent.Replace(mediaObject.FileName, mediaObject.FileName);
                    var file = ConvertBase64ToIFormFile(mediaObject.Base64Src, mediaObject.FileName);
                    if (file != null)
                        files.Add(file);
                } else
                if (mediaObject.FileName.EndsWith(".txt"))
                {
                    // Извлекаем Base64 часть и декодируем
                    var file = ConvertBase64ToIFormFile(mediaObject.Base64Src, mediaObject.FileName);
                    if (file != null)
                        files.Add(file);

                    // Заменяем на декодированный текст
                    htmlContent = htmlContent.Replace(mediaObject.FileName, mediaObject.FileName);
                }
                else
                {
                    htmlContent = htmlContent.Replace(mediaObject.FileName, $"<p>{mediaObject.Base64Src}</p>");
                }
            }

            return (htmlContent,files);
        }

        private string GenerateHtml(ReceivedData data)
        {
            string html = @"
        <html>
        <head>
            <title>Заявка</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f9;
                    color: #333;
                    padding: 20px;
                }
                h1 {
                    font-size: 24px;
                    color: #2C3E50;
                }
                h3 {
                    font-size: 18px;
                    color: #34495E;
                    margin-top: 20px;
                }
                h4 {
                    font-size: 16px;
                    color: #7F8C8D;
                    margin-top: 10px;
                }
                ul {
                    padding-left: 20px;
                }
                li {
                    font-size: 14px;
                    margin-bottom: 5px;
                }
                .question {
                    font-weight: bold;
                    color: #2980B9;
                }
                .answer {
                    margin-left: 10px;
                    color: #27AE60;
                }
                .content-container {
                    background-color: white;
                    padding: 20px;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                }
            </style>
        </head>
        <body>
            <div class='content-container'>
        ";

            foreach (var topicGroup in data.receivedTopics)
            {
                // Генерация заголовков
                foreach (var topic in topicGroup.topics)
                {
                    html += $"<{topic.tag.ToLower()}>{topic.text}</{topic.tag.ToLower()}>\n";
                }

                // Генерация вопросов и ответов
                foreach (var item in topicGroup.receivedTopics)
                {
                    html += $"<h4>{item.question}</h4>\n<ul>\n";
                    foreach (var answer in item.answer)
                    {
                        if (answer.StartsWith("^"))
                        {
                            html += $"<p>{answer.TrimStart("^")}</p>\n";
                        }
                        else
                        {
                            html += $"<li>{answer}</li>\n";
                        }

                    }
                    html += "</ul>\n";


                }
            }

            html += "</body>\n</html>";
            return html;
        }

        private void SaveData(string data)
        {
            var udi = _contentService.GetById(1254).GetUdi(); 
            var request = _contentService.CreateContent("Заявка", udi, "request");
            request.SetValue("jsonRequest", data);
            //_contentService.Save(request);
        }

        [IgnoreAntiforgeryToken]
        [HttpGet]
        public async Task<ActionResult> GetFormModel()
        {
            HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");

            var homePage = _Umbraco.ContentAtRoot().FirstOrDefault(x => x.ContentType.Alias == "homePage");
            var formMainPage = homePage.Children.FirstOrDefault(x => x.ContentType.Alias == "formMainPage");

            standartFormRow standartFormRow = new standartFormRow();
            BlockByForm BlokByForm = new BlockByForm();
            CategoryForm CategoryForm = new CategoryForm();
            FormModel FormModel = new FormModel();
            rowBlock rowBlock = new rowBlock();
            if (formMainPage != null)
            {
                FormModel = new FormModel();
                FormModel.Name = formMainPage.Value("title").ToString() == "" ? formMainPage.Name : formMainPage.Value("title").ToString();
                FormModel.Description = formMainPage.Value("description").ToString();
                foreach (var child in formMainPage.Children)
                {

                    var blocksContent = child.Value("formPageConstructor") as Umbraco.Cms.Core.Models.Blocks.BlockListModel;

                    if (blocksContent != null)
                    {
                        CategoryForm = new CategoryForm();
                        CategoryForm.Name = child.Value("title").ToString() == "" ? child.Name : child.Value("title").ToString();
                        CategoryForm.Title = child.Value("title").ToString() != "" ? child.Value("title").ToString() : "";
                        CategoryForm.Description = child.Value("description").ToString();
                        var blocksList = blocksContent.ToList();
                        BlokByForm = new BlockByForm();
                        foreach (var block in blocksList)
                        {


                            if (((PublishedElementWrapped)block.Content).ContentType.Alias == "formTextEditor")
                            {
                                var TextEditor = ((FormTextEditor)((PublishedElementWrapped)block.Content)).TextEditor.ToString();
                                if (TextEditor != null && TextEditor != "")
                                {
                                    formTextEditor formTextEditor = new formTextEditor()
                                    {
                                        alias = "TextEditor",
                                        TextEditor = TextEditor
                                    };
                                    BlokByForm.AddElement(formTextEditor);
                                }

                            }
                            else if (((PublishedElementWrapped)block.Content).ContentType.Alias == "standartFormRow")
                            {
                                var blockContent = ((BlockListItem<StandartFormRow>)block).Content;
                                if (blockContent.FormRowItem.Any())
                                {
                                    standartFormRow = new standartFormRow();
                                    standartFormRow.alias = "standartFormRow";
                                    foreach (var formRow in blockContent.FormRowItem)
                                    {

                                        if (formRow.Areas != null)
                                        {
                                            rowBlock = new rowBlock();
                                            foreach (var row in formRow.Areas.ToList())
                                            {

                                                if (row != null)
                                                {

                                                    foreach (var item in row.ToList())
                                                    {

                                                        if (((PublishedContentType)((PublishedElementWrapped)item.Content).ContentType).Alias == "umbBlockGridDemoHeadlineBlock")
                                                        {
                                                            string Headline = ((BlockGridItem<UmbBlockGridDemoHeadlineBlock>)item).Content.Headline;
                                                            string HeaderHint = ((BlockGridItem<UmbBlockGridDemoHeadlineBlock>)item).Content.HeaderHint.ToString();
                                                            umbBlockGridDemoHeadlineBlock umbBlockGridDemoHeadlineBlock = new umbBlockGridDemoHeadlineBlock()
                                                            {
                                                                alias = "umbBlockGridDemoHeadlineBlock",
                                                                HeaderHint = HeaderHint,
                                                                Headline = Headline
                                                            };
                                                            rowBlock.AddElement(umbBlockGridDemoHeadlineBlock);
                                                        }
                                                        else if (((PublishedContentType)((PublishedElementWrapped)item.Content).ContentType).Alias == "label")
                                                        {
                                                            string LabelHint = ((BlockGridItem<Label>)item).Content.LabelHint.ToString();
                                                            string LabelNotise = ((BlockGridItem<Label>)item).Content.LabelNotise.ToString();
                                                            string LabelText = ((BlockGridItem<Label>)item).Content.LabelText.ToString();
                                                            label label = new label()
                                                            {
                                                                alias = "label",
                                                                LabelHint = LabelHint,
                                                                LabelNotise = LabelNotise,
                                                                LabelText = LabelText
                                                            };
                                                            rowBlock.AddElement(label);
                                                        }
                                                        else if (((PublishedContentType)((PublishedElementWrapped)item.Content).ContentType).Alias == "formInput")
                                                        {
                                                            string ProretyType = ((BlockGridItem<FormInput>)item).Content.PropertyType;
                                                            string PropertyAlias = ((BlockGridItem<FormInput>)item).Content.PropertyAlias;
                                                            bool PropertyRequired = ((BlockGridItem<FormInput>)item).Content.PropertyRequired;
                                                            string PropertyName = ((BlockGridItem<FormInput>)item).Content.PropertyName;
                                                            formInput formInput = new formInput
                                                            {
                                                                alias = "formInput",
                                                                PropertyAlias = PropertyAlias,
                                                                PropertyRequired = PropertyRequired,
                                                                PropertyName = PropertyName,
                                                                ProretyType = ProretyType
                                                            };
                                                            rowBlock.AddElement(formInput);
                                                        }
                                                        else if (((PublishedContentType)((PublishedElementWrapped)item.Content).ContentType).Alias == "formSelect")
                                                        {
                                                            var ListSelects = ((BlockGridItem<FormSelect>)item).Content.SelectOptions;
                                                            var PropertyName = ((BlockGridItem<FormSelect>)item).Content.PropertyName;
                                                            var PropertyAlias = ((BlockGridItem<FormSelect>)item).Content.PropertyAlias;
                                                            formSelect formSelect = new formSelect()
                                                            {

                                                                alias = "formSelect",
                                                                PropertyAlias = PropertyAlias,
                                                                PropertyName = PropertyName,
                                                            };
                                                            var a = "";
                                                            if (ListSelects.Any())
                                                            {
                                                                var selectOptionsList = new List<SelectOptions>();
                                                                foreach (var selectItem in ListSelects)
                                                                {
                                                                    SelectOptions listSelect = new SelectOptions();
                                                                    var selectText = ((Umbraco.Cms.Core.Models.Blocks.BlockListItem<Umbraco.Cms.Web.Common.PublishedModels.UniTextString>)selectItem).Content.TextStiring;
                                                                    listSelect.Name = selectText;
                                                                    selectOptionsList.Add(listSelect);
                                                                }
                                                                formSelect.selectOptions = selectOptionsList;
                                                            }
                                                            rowBlock.AddElement(formSelect);
                                                        }
                                                        else
                                                        {

                                                        }


                                                    }

                                                }

                                            }
                                            standartFormRow.rowBlocks.Add(rowBlock);
                                        }
                                    }
                                }
                                BlokByForm.AddElement(standartFormRow);
                            }
                            else if (((PublishedElementWrapped)block.Content).ContentType.Alias == "btnSubmit")
                            {
                                string NameButton = ((BtnSubmit)(((PublishedElementWrapped)block.Content))).BtnText;
                                btnSubmit btnSubmit = new btnSubmit()
                                {
                                    alias = "btnSubmit",
                                    NameButton = NameButton
                                };
                                BlokByForm.AddElement(btnSubmit);
                            }
                        }

                        CategoryForm.blokByForms.Add(BlokByForm);
                    }
                    FormModel.CategoryForms.Add(CategoryForm);
                }
            }

            return Ok(JsonConvert.SerializeObject(FormModel));
        }
        public class ReceivedData
        {
            public List<ReceivedTopic> receivedTopics = new List<ReceivedTopic>(); 
        }
        public class ReceivedTopic
        {
            public List<topic> topics = new List<topic>();
            public List<ReceivedItem> receivedTopics = new List<ReceivedItem>();
        }
        public class topic
        {
            public string tag { get; set; }
            public string text { get; set; }

        }
        public class ReceivedItem
        {
            public string question { get; set; }
            public HashSet<string> answer = new HashSet<string>();
        }

        public class FormModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<CategoryForm> CategoryForms = new List<CategoryForm>();
        }
        public class CategoryForm
        {
            public string Name { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public List<BlockByForm> blokByForms = new List<BlockByForm>();
        }

        public class BlockByForm
        {
            public string alias { get; set; }
            public List<dynamic> formElements = new List<dynamic>();
            public void AddElement(dynamic element)
            {
                formElements.Add(element);
            }
        }
        public class btnSubmit
        {
            public string NameButton { get; set; }
            public string alias { get; set; }
        }
        public class formTextEditor
        {
            public string alias { get; set; }
            public string TextEditor { get; set; }
        }
        public class standartFormRow
        {
            public string alias { get; set; }
            public List<rowBlock> rowBlocks = new List<rowBlock>();
        }
        public class rowBlock
        {
            public List<dynamic> rowElements = new List<dynamic>();
            public void AddElement(dynamic element)
            {
                rowElements.Add(element);
            }
        }
        public class umbBlockGridDemoHeadlineBlock
        {
            public string alias { get; set; }
            public string Headline { get; set; }
            public string HeaderHint { get; set; }

        }
        public class label
        {
            public string alias { get; set; }
            public string LabelHint { get; set; }
            public string LabelNotise { get; set; }
            public string LabelText { get; set; }
        }
        public class formInput
        {
            public string alias { get; set; }
            public string ProretyType { get; set; }
            public string PropertyAlias { get; set; }
            public bool PropertyRequired { get; set; }
            public string PropertyName { get; set; }
        }
        public class formSelect
        {
            public string alias { get; set; }
            public string PropertyName { get; set; }
            public string PropertyAlias { get; set; }
            public List<SelectOptions> selectOptions = new List<SelectOptions>();
        }
        public class SelectOptions
        {
            public string Name { get; set; }
        }
        public class ImageObject
        {
            public string FileName { get; set; }
            public string Base64Src { get; set; }
        }
    }
}
