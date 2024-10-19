using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ClubSite
{
    /// <summary>
    /// Результат сабмита формы через Ajax.
    /// </summary>
    public class FormAjaxPostResult
    {
        public bool IsSuccess { get; set; }
        public string? FailMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public string? RedirectTo { get; set; }
        public Dictionary<string, string>? Errors { get; set; }
        public object? ExtraData { get; set; }

        public static FormAjaxPostResult Success(string? successMessage = null, object? extraData = null, string? redirectTo = null)
        {
            return new FormAjaxPostResult
            {
                IsSuccess = true,
                SuccessMessage = successMessage,
                ExtraData = extraData,
                RedirectTo = redirectTo,
            };
        }

        public static FormAjaxPostResult Fail(string? failMessage = null, object? extraData = null)
        {
            return new FormAjaxPostResult
            {
                IsSuccess = false,
                FailMessage = failMessage,
                ExtraData = extraData
            };
        }

        public static FormAjaxPostResult Fail(ModelStateDictionary modelState, object? extraData = null)
        {
            var res = new FormAjaxPostResult() { IsSuccess = false };

            res.FailMessage = "Обнаружены ошибки заполнения";

            if (modelState != null && modelState.Keys.Any())
            {
                res.Errors = new Dictionary<string, string>();
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                foreach (var field in modelState.Keys)
                {
                    if (modelState[field].Errors.Any())
                        res.Errors[field] = modelState[field].Errors.First().ErrorMessage;
                }
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
            }

            res.ExtraData = extraData;

            return res;
        }
    }
}