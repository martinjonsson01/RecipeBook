using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers;

using RecipeBook.Core.Domain;

namespace RecipeBook.Core.Application.Logic
{
    public class InputSaver<TResource>
        where TResource : IShallowCloneable<TResource>
    {
        public InputSaver(
            TResource                    resource,
            HttpClient                   http,
            string                       apiPutUrl,
            Action<(string, LoadStatus)> setSaving)
        {
            _resource = resource;
            _http = http;
            _setSaving = setSaving;
            _apiPutUrl = apiPutUrl;
            UpdateLastSaved(true);
            _inputTimer.Elapsed += CheckInput;
            _inputTimer.Start();
        }

        public event EventHandler Saved = null!;

        private const int    CheckInputInterval   = 100;
        private const int    SaveToServerInterval = 1000;
        private const string InputSaveTag         = "input";
        private const string SaveToServerTag      = "save to server";

        private readonly TResource _resource;
        private readonly HttpClient _http;
        private readonly Action<(string, LoadStatus)> _setSaving;
        private readonly Timer _inputTimer = new() { Interval = CheckInputInterval, AutoReset = true };
        private readonly string _apiPutUrl;

        private TResource? _lastSavedResource;
        private DateTime   _lastInputSave = DateTime.MinValue;

        public void ResourceHasChanged()
        {
            _setSaving((InputSaveTag, LoadStatus.None)); // To indicate that displayed data is not saved
        }

        private async void CheckInput(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Subtract(_lastInputSave) <= TimeSpan.FromMilliseconds(SaveToServerInterval)) return;
            if (_resource.Equals(_lastSavedResource)) return;

            await SaveResource();
        }

        private async Task SaveResource()
        {
            UpdateLastSaved();
            _setSaving((InputSaveTag, LoadStatus.Loading));
            _setSaving((SaveToServerTag, LoadStatus.Loading));
            HttpResponseMessage response = await _http.PutAsJsonAsync(_apiPutUrl, _resource);
            LoadStatus          status   = response.IsSuccessStatusCode ? LoadStatus.Success : LoadStatus.Fail;
            Saved.Invoke(this, EventArgs.Empty);
            _setSaving((SaveToServerTag, status));
            _setSaving((InputSaveTag, status));
        }

        private void UpdateLastSaved(bool init = false)
        {
            if (!init) _lastInputSave = DateTime.Now;
            _lastSavedResource = _resource.ShallowClone();
        }
    }
}