using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using RecipeBook.Core.Application.Logic;
using RecipeBook.Core.Domain;

namespace RecipeBook.Presentation.WebApp.Client.Shared
{
    public partial class FetchList<TItem>
        where TItem : BaseEntity
    {
        private ObservableCollection<TItem>? _items;

        private HttpStatusCode? _responseStatus;

        protected override async Task OnInitializedAsync()
        {
            _items = null;
            _responseStatus = null;

            HttpResponseMessage response = await _http.GetAsync(Url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var serializerOptions = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                string json = await response.Content.ReadAsStringAsync();
                _items = JsonConvert.DeserializeObject<ObservableCollection<TItem>>(json, serializerOptions);

                if (_items is not null)
                    _items.CollectionChanged += ItemsCollectionChanged;

                return;
            }
            _responseStatus = response.StatusCode;
        }

        private async void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    await ItemsAdded(e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    await ItemsRemoved(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e));
            }
        }


        private async Task ItemsAdded(IList? items)
        {
            if (items is null) return;
            foreach (TItem addedItem in items)
                await UploadNewItem(addedItem);
        }

        private async Task ItemsRemoved(IList? items)
        {
            if (items is null) return;
            foreach (TItem removedItem in items)
                await DeleteItem(removedItem);
        }

        private async Task UploadNewItem(TItem item)
        {
            JsonSerializerSettings serializerOptions = new() { TypeNameHandling = TypeNameHandling.Auto };
            StringContent          content           = ItemToStringContent(item, serializerOptions);

            await SendHttpMessageWithSetSaving(
                $"{typeof(TItem).Name}-UploadNew",
                () => _http.PutAsync(Url, content),
                response => UpdateItemId(item, response, serializerOptions));
        }

        private async Task DeleteItem(TItem item)
        {
            await SendHttpMessageWithSetSaving(
                $"{typeof(TItem).Name}-DeleteItem",
                () => _http.DeleteAsync($"{Url}/{item.Id}"),
                null);
        }

        private async Task<HttpResponseMessage> SendHttpMessageWithSetSaving(
            string                           saveTaskName,
            Func<Task<HttpResponseMessage>>  httpAction,
            Func<HttpResponseMessage, Task>? successAction)
        {
            await SetSaving.InvokeAsync((saveTaskName, LoadStatus.Loading));
            HttpResponseMessage response = await httpAction();
            await HandleResponse(response, saveTaskName, successAction);
            return response;
        }


        private async Task UpdateItemId(
            TItem                  item,
            HttpResponseMessage    response,
            JsonSerializerSettings serializerOptions)
        {
            string responseJson = await response.Content.ReadAsStringAsync();
            var    responseItem = JsonConvert.DeserializeObject<TItem>(responseJson, serializerOptions);
            item.Id = responseItem?.Id;
        }

        private async Task HandleResponse(
            HttpResponseMessage              response,
            string                           taskName,
            Func<HttpResponseMessage, Task>? successAction)
        {
            if (response.IsSuccessStatusCode)
            {
                await SetSaving.InvokeAsync((taskName, LoadStatus.Success));
                if (successAction is not null)
                    await successAction(response);
            }
            else
                await SetSaving.InvokeAsync((taskName, LoadStatus.Fail));
        }

        private static StringContent ItemToStringContent(TItem item, JsonSerializerSettings? serializerOptions)
        {
            string json    = JsonConvert.SerializeObject(item, serializerOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}