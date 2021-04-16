using System;
using System.Net.Http;

namespace RecipeBook.Core.Application.Logic
{
    public class InputSavedEventArgs : EventArgs
    {
        public InputSavedEventArgs(HttpResponseMessage response)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; }
    }
}