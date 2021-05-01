using System;
using System.Net.Http;
using System.Threading.Tasks;

using RecipeBook.Core.Application.Logic;

namespace RecipeBook.Core.Application.Web
{
    public static class HttpHelper
    {
        public static async Task<HttpResponseMessage> SendHttpMessageWithSetSaving(
            string                           saveTaskName,
            Func<Task<HttpResponseMessage>>  httpAction,
            Func<HttpResponseMessage, Task>? successAction,
            Func<(string, LoadStatus), Task> setSaving)
        {
            await setSaving((saveTaskName, LoadStatus.Loading));
            HttpResponseMessage response = await httpAction();
            await HandleResponse(response, saveTaskName, successAction, setSaving);
            return response;
        }

        private static async Task HandleResponse(
            HttpResponseMessage              response,
            string                           taskName,
            Func<HttpResponseMessage, Task>? successAction,
            Func<(string, LoadStatus), Task> setSaving)
        {
            if (response.IsSuccessStatusCode)
            {
                await setSaving((taskName, LoadStatus.Success));
                if (successAction is not null)
                    await successAction(response);
            }
            else
                await setSaving((taskName, LoadStatus.Fail));
        }
    }
}