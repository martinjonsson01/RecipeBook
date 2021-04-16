using System;
using System.Net.Http;

using Microsoft.AspNetCore.Components;

using RecipeBook.Core.Application.Logic;
using RecipeBook.Core.Domain;

namespace RecipeBook.Presentation.WebApp.Client.Pages.Recipe
{
    public partial class SyncedView<TResource> : ComponentBase
        where TResource : IShallowCloneable<TResource>, IEquatable<TResource>
    {
        [Inject] private HttpClient Http { get; set; } = null!;

        [Parameter] public string Url { get; set; } = null!;

        [Parameter] public TResource Step { get; set; } = default!;

        [Parameter]
        public EventCallback<(string, LoadStatus)> SetSaving { get; set; } = EventCallback<(string, LoadStatus)>.Empty;

        protected override void OnInitialized()
        {
            _inputSaver = new InputSaver<TResource>(Step, Http, Url, SetSavingDelegate);
        }

        protected void ResourceHasChanged() => _inputSaver.ResourceHasChanged();
        
        private void SetSavingDelegate((string, LoadStatus) tuple) => SetSaving.InvokeAsync(tuple);

        private InputSaver<TResource> _inputSaver = null!;
    }
}