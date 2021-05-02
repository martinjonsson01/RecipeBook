using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using Newtonsoft.Json;

using RecipeBook.Core.Application.Logic;
using RecipeBook.Core.Application.Web;
using RecipeBook.Core.Domain;

namespace RecipeBook.Presentation.WebApp.Client.Shared
{
    public partial class FetchList<TItem> : IHasFocusedItem<TItem>
        where TItem : BaseEntity
    {
        private ObservableCollection<TItem>? _items;

        private HttpStatusCode? _responseStatus;

        public TItem? FocusedItem { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            await ReloadItems();
        }

        public async Task ReloadItems()
        {
            _items = null;
            _responseStatus = null;

            HttpResponseMessage response = await _http.GetAsync(Url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var serializerOptions = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    };

                    string json = await response.Content.ReadAsStringAsync();
                    _items = JsonConvert.DeserializeObject<ObservableCollection<TItem>>(json, serializerOptions);
                    break;
                case HttpStatusCode.NoContent:
                    _items = new ObservableCollection<TItem>();
                    break;
            }

            if (_items is not null)
                _items.CollectionChanged += ItemsCollectionChanged;
            
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
                    // No need to do anything here since items themselves synchronize values such as Number.
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e));
            }
            StateHasChanged();
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

            await HttpHelper.SendHttpMessageWithSetSaving(
                $"{typeof(TItem).Name}-UploadNew",
                () => _http.PutAsync(Url, content),
                async response => await OnUploadItemSuccess(response, item, serializerOptions),
                tuple => SetSaving.InvokeAsync(tuple));
        }

        private async Task DeleteItem(TItem item)
        {
            await HttpHelper.SendHttpMessageWithSetSaving(
                $"{typeof(TItem).Name}-DeleteItem",
                () => _http.DeleteAsync($"{Url}/{item.Id}"),
                null,
                tuple => SetSaving.InvokeAsync(tuple));
        }

        private async Task OnUploadItemSuccess(HttpResponseMessage response, TItem item, JsonSerializerSettings serializerOptions)
        {
             await UpdateItemId(item, response, serializerOptions);
             FocusedItem = item;
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


        private static StringContent ItemToStringContent(TItem item, JsonSerializerSettings? serializerOptions)
        {
            string json    = JsonConvert.SerializeObject(item, serializerOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}