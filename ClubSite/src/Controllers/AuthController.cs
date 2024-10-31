using Examine;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Website.Controllers;

namespace ClubSite.src.Controllers
{
    public class AuthController : SurfaceController
    {
        private readonly IMemberService _memberService;
        private readonly IMemberManager _memberManager;
        private readonly SignInManager<MemberIdentityUser> _signInManager;
        UmbracoHelper Umbraco { get; set; }
        IExamineManager ExamineManager { get; set; }
        public AuthController(ILogger<CustomFormController> logger, IMemberService memberService, IMemberManager memberManager, SignInManager<MemberIdentityUser> signInManager,
            UmbracoHelper umbracoHelper,
            IExamineManager examineManager,
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberService = memberService;
            _memberManager = memberManager;
            _signInManager = signInManager;
            this.ExamineManager = examineManager;
            this.Umbraco = umbracoHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(new { success = true, message = "Вход выполнен успешно!" });
            }

            return BadRequest(new { success = false, message = "Неверные данные для входа" });
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            // Проверка на существование пользователя с таким же именем
            if (_memberService.GetByUsername(username) != null)
            {
                return BadRequest(new { success = false, message = "Пользователь с таким именем уже существует" });
            }

            // Проверка на существование пользователя с таким же email
            var existingUserByEmail = await _memberManager.FindByEmailAsync(email);
            if (existingUserByEmail != null)
            {
                return BadRequest(new { success = false, message = "Пользователь с таким email уже существует" });
            }

            // Создание нового пользователя
            var identityUser = MemberIdentityUser.CreateNew(
                username,
                email,
                "Member", // Тип члена (поменяйте, если у вас другой тип)
                true, // Активен
                username // Полное имя (можно использовать model.Name)
            );
            var identityResult = await _memberManager.CreateAsync(identityUser, password);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                return BadRequest(new { success = false, message = $"Ошибка при создании пользователя: {errors}" });
            }

            // Проверка, что пароль корректно сохранен
            var createdUser = await _memberManager.FindByNameAsync(username);
            var isPasswordValid = await _memberManager.CheckPasswordAsync(createdUser, password);

            if (!isPasswordValid)
            {
                return BadRequest(new { success = false, message = "Пароль сохранен некорректно" });
            }

            return Ok(new { success = true, message = "Регистрация успешна!" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { success = true, message = "Выход выполнен успешно!" });
        }
    }
}
