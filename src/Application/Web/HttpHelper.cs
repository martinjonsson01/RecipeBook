using System;
using System.Net.Http;
using System.Threading.Tasks;

using RecipeBook.Core.Application.Logic;

namespace RecipeBook.Core.Application.Web
{
    public static class HttpHelper
    {
        public static async Task SendHttpMessageWithSetSaving(
            string                          saveTaskName,
            Func<Task<HttpResponseMessage>> httpAction,
            Action<HttpResponseMessage>?    successAction,
            Action<(string, LoadStatus)>    setSaving)
        {
            setSaving((saveTaskName, LoadStatus.Loading));
            HttpResponseMessage response = await httpAction();
            HandleResponse(response, saveTaskName, successAction, setSaving);
        }

        private static void HandleResponse(
            HttpResponseMessage          response,
            string                       taskName,
            Action<HttpResponseMessage>? successAction,
            Action<(string, LoadStatus)> setSaving)
        {
            if (response.IsSuccessStatusCode)
            {
                setSaving((taskName, LoadStatus.Success));
                successAction?.Invoke(response);
            }
            else
                setSaving((taskName, LoadStatus.Fail));
        }
    }
}