using ClubSite;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace ClubSite
{
    [PluginController("theme")]
    public class ThemeSelectController : UmbracoAuthorizedJsonController
    {
        public ObjectResult GetAllThemes()
        {
            var result = new List<string>();
            var pathToFolder = SiteHelpers.MapPath("css");
            var themeFolders = Directory.GetDirectories(pathToFolder);
            foreach (string folder in themeFolders)
            {
                var folderName = folder.Split('\\').LastOrDefault()!;
                if(folderName.StartsWith("theme-"))
                    result.Add(folderName.Substring("theme-".Length));
            }
            return Ok(result);
        }

        [HttpPost]
        public ObjectResult CopyDirectory(CopyDirReq req)
        {
            if (req == null)
                return BadRequest("Тело запроса не может быть пустым.");

            if (req.OldFolderName == null)
                return BadRequest("Не задана исходная тема для копирования.");

            if (req.NewFolderName == null)
                return BadRequest("Укажите название новой темы.");

            try
            {
                var pathToFolderCss = SiteHelpers.MapPath("css");
                var themeFolders = Directory.GetDirectories(pathToFolderCss);
                foreach (string folder in themeFolders)
                {
                    var _folder = folder.Split('\\').LastOrDefault()!;
                    if (_folder.StartsWith("theme-"))
                    {
                        _folder = _folder.Substring("theme-".Length);
                        if (_folder == req.OldFolderName)
                        {
                            CopyDirectory(folder,
                                SiteHelpers.MapPath($"css/theme-{req.NewFolderName}"),
                                true);
                        }
                    }
                }

                var pathToFolderJs = SiteHelpers.MapPath("scripts");
                var allfoldersJs = Directory.GetDirectories(pathToFolderJs);
                foreach (string folder in allfoldersJs)
                {
                    var _folder = folder.Split('\\').LastOrDefault()!;
                    if (_folder.StartsWith("theme-"))
                    {
                        _folder = _folder.Substring("theme-".Length);
                        if (_folder == req.OldFolderName)
                        {
                            CopyDirectory(folder,
                                SiteHelpers.MapPath($"scripts/theme-{req.NewFolderName}"),
                                true);
                        }
                    }
                }
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok("Копия темы успешно создана");
        }

        private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Получить информацию об исходном каталоге
            var dir = new DirectoryInfo(sourceDir);

            // Проверка, существует ли исходный каталог
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Исходный каталог не найден: {dir.FullName}");

            // Кэширует каталоги, прежде чем начать копирование
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Создает новый каталог
            Directory.CreateDirectory(destinationDir);

            // Получаем файлы в исходном каталоге и копируем в целевой каталог
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // Если рекурсивно и копируются подкаталоги
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public class CopyDirReq
        {
            public string? OldFolderName { get; set; }
            public string? NewFolderName { get; set; }
        }
    }
}