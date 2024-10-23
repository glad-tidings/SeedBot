using System.Net;

namespace Seed
{

    public class SeedApi
    {
        private readonly HttpClient client;

        public SeedApi(string queryID, int queryIndex, ProxyType[] Proxy)
        {
            var FProxy = Proxy.Where(x => x.Index == queryIndex);
            if (FProxy.Count() != 0)
            {
                if (!string.IsNullOrEmpty(FProxy.ElementAtOrDefault(0)?.Proxy))
                {
                    var handler = new HttpClientHandler() { Proxy = new WebProxy() { Address = new Uri(FProxy.ElementAtOrDefault(0)?.Proxy ?? string.Empty) } };
                    client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 30) };
                }
                else
                {
                    client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
                }
            }
            else
            {
                client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            }
            client.DefaultRequestHeaders.Add("Telegram-Data", $"{queryID}");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Origin", "https://cf.seeddao.org");
            client.DefaultRequestHeaders.Add("Referer", "https://cf.seeddao.org/");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
            client.DefaultRequestHeaders.Add("User-Agent", Tools.getUserAgents(queryIndex));
            client.DefaultRequestHeaders.Add("accept", "*/*");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", $"\"{Tools.getUserAgents(queryIndex, true)}\"");
        }

        public async Task<HttpResponseMessage> SAPIGet(string requestUri)
        {
            try
            {
                return await client.GetAsync(requestUri);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.ExpectationFailed, ReasonPhrase = ex.Message };
            }
        }

        public async Task<HttpResponseMessage> SAPIPost(string requestUri, HttpContent content)
        {
            try
            {
                return await client.PostAsync(requestUri, content);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.ExpectationFailed, ReasonPhrase = ex.Message };
            }
        }
    }
}