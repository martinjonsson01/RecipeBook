using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Newtonsoft.Json;

using RecipeBook.Core.Domain;

namespace RecipeBook.Core.Application.Logic
{
    public class InputSaver<TResource>
        where TResource : BaseEntity, IShallowCloneable<TResource>, IEquatable<TResource>
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
            Saved += OnSaved;
        }

        public event EventHandler<InputSavedEventArgs> Saved = null!;

        private const int    CheckInputInterval   = 100;
        private const int    SaveToServerInterval = 1000;
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
            _setSaving((ResourceIdentifier, LoadStatus.None)); // To indicate that displayed data is not saved
        }

        private string ResourceIdentifier => $"{typeof(TResource).FullName}:{_resource.Id}";

        private async void CheckInput(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Subtract(_lastInputSave) <= TimeSpan.FromMilliseconds(SaveToServerInterval)) return;
            if (_resource.Equals(_lastSavedResource)) return;

            await SaveResource();
        }

        private async Task SaveResource()
        {
            UpdateLastSaved();
            _setSaving((ResourceIdentifier, LoadStatus.Loading));
            _setSaving((SaveToServerTag, LoadStatus.Loading));

            string json = JsonConvert.SerializeObject(_resource,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _http.PutAsync(_apiPutUrl, content);

            Saved.Invoke(this, new InputSavedEventArgs(response));
        }

        private void OnSaved(object? sender, InputSavedEventArgs e)
        {
            LoadStatus status = e.Response.IsSuccessStatusCode ? LoadStatus.Success : LoadStatus.Fail;
            _setSaving((SaveToServerTag, status));
            _setSaving((ResourceIdentifier, status));
        }

        private void UpdateLastSaved(bool init = false)
        {
            if (!init) _lastInputSave = DateTime.Now;
            _lastSavedResource = _resource.ShallowClone();
        }
    }
}