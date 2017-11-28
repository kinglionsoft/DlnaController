using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SV.UPnPLite
{
    public static class HttpClientHelper
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private const string DefaultContentType = "text/xml";
        private static HttpClient _httpClient;

        static HttpClientHelper()
        {
            _httpClient = new HttpClient();
        }

        public static Task<Stream> PostXmlAsync(string url, string xml, Dictionary<string, string> headers = null)
        {
            return PostXmlAsync(new Uri(url), xml, headers);
        }

        public static async Task<Stream> PostXmlAsync(Uri uri, string xml, Dictionary<string, string> headers = null)
        {
            var content = new StringContent(xml, DefaultEncoding, DefaultContentType);
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    content.Headers.Add(h.Key, h.Value);
                }
            }

            var response = await _httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        public static async Task<Stream> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }
}