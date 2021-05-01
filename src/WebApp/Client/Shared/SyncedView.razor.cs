using System;
using System.Net.Http;

using Microsoft.AspNetCore.Components;

using RecipeBook.Core.Application.Logic;
using RecipeBook.Core.Domain;

namespace RecipeBook.Presentation.WebApp.Client.Shared
{
    public partial class SyncedView<TResource> : ComponentBase
        where TResource : BaseEntity, IShallowCloneable<TResource>, IEquatable<TResource>
    {
        [Inject] private HttpClient Http { get; set; } = null!;

        [Parameter] public string Url { get; set; } = null!;

        [Parameter]
        public EventCallback<(string, LoadStatus)> SetSaving { get; set; } = EventCallback<(string, LoadStatus)>.Empty;

        public void Initialize(TResource resource)
        {
            _inputSaver = new InputSaver<TResource>(resource, Http, Url, SetSavingDelegate);
            _inputSaver.Saved += (sender, args) => Synced?.Invoke(sender, args);
        }

        public void ResourceHasChanged() => _inputSaver.ResourceHasChanged();

        protected event EventHandler<InputSavedEventArgs>? Synced;
        
        protected virtual void SetSavingDelegate((string, LoadStatus) tuple) => SetSaving.InvokeAsync(tuple);

        private InputSaver<TResource> _inputSaver = null!;

    }
}