using Microsoft.JSInterop;

namespace BlazorVault
{
    /*
     * This class is used as a service to copy text to the clipboard.
     * Functional even on iOS devices.
     */
    public class ClipboardService(IJSRuntime jsRuntime)
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));

        public async Task<bool> CopyToClipboardAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Impossible de copier un texte vide dans votre presse-papiers.");
            }
            try
            {
                await _jsRuntime.InvokeVoidAsync("copyTextToClipboard", text);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Impossible de copier le texte dans votre presse-papiers.", ex);
            }
        }
    }
}
