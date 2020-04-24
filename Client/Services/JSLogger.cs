using Microsoft.JSInterop;

namespace csharpwebsite.Client.Services
{
    public class JSLogger
    {
        private readonly IJSRuntime _jsRuntime;

        public JSLogger(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public void log(object obj)
        {
#if DEBUG
            ((IJSInProcessRuntime)_jsRuntime).Invoke<object>("log", obj);
#endif
        }
    }
}