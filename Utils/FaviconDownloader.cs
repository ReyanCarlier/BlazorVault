namespace BlazorVault.Utils
{
    /*
     *  This class is used to download the favicon of a website.
     *  It uses the Google favicon API to download the favicon.
     *  The favicon is then stored in the database as a byte array.
     */
    public class FaviconDownloader(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<byte[]> DownloadFaviconAsync(string domain)
        {
            byte[] faviconBytes = [];
            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentException("Domain must not be empty", nameof(domain));
            }
            try
            {
                string faviconUrl = $"https://www.google.com/s2/favicons?domain={domain}&size=16";
                var response = await _httpClient.GetAsync(faviconUrl);
                if (response.IsSuccessStatusCode)
                {
                    faviconBytes = await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    Console.WriteLine($"Error downloading favicon for {domain}: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading favicon for {domain}: {ex.Message}");
            }
            return faviconBytes;
        }
    }
}
