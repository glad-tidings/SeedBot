using System.Net;
using System.Text;
using System.Text.Json;

namespace Seed
{

    public class SeedBot
    {

        public readonly SeedQuery PubQuery;
        private readonly ProxyType[] PubProxy;
        public readonly SeedProfile2Response UserDetail;
        public readonly bool HasError;
        public readonly string ErrorMessage;
        public readonly string IPAddress;

        public SeedBot(SeedQuery Query, ProxyType[] Proxy)
        {
            PubQuery = Query;
            PubProxy = Proxy;
            IPAddress = GetIP().Result;
            PubQuery.Auth = getSession();
            var GetUserDetail = SeedGetProfile2().Result;
            if (GetUserDetail is not null)
            {
                UserDetail = GetUserDetail;
                HasError = false;
                ErrorMessage = "";
            }
            else
            {
                UserDetail = new();
                HasError = true;
                ErrorMessage = "get profile data failed";
            }
        }

        private async Task<string> GetIP()
        {
            HttpClient client;
            var FProxy = PubProxy.Where(x => x.Index == PubQuery.Index);
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
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync($"https://httpbin.org/ip");
            }
            catch { }
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<Httpbin>(responseStream);
                    return responseJson?.Origin ?? string.Empty;
                }
            }

            return "";
        }

        private string getSession()
        {
            var vw = new TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "seed_coin_bot", "https://cf.seeddao.org/");
            string url = vw.Get_URL().Result;

            if (url != string.Empty)
            {
                return url.Split(new string[] { "tgWebAppData=" }, StringSplitOptions.None)[0].Split(new string[] { "&tgWebAppVersion" }, StringSplitOptions.None)[0];
            }

            return string.Empty;
        }

        private async Task<SeedProfile2Response?> SeedGetProfile2()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIGet("https://elb.seeddao.org/api/v1/profile2");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<SeedProfile2Response>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<SeedBalanceResponse?> SeedGetBalance()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIGet("https://elb.seeddao.org/api/v1/profile/balance");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<SeedBalanceResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<SeedStreakRewardResponse?> SeedDailyReward()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var loginBonuses = await SAPI.SAPIPost("https://elb.seeddao.org/api/v1/login-bonuses", null);
            if (loginBonuses is not null)
            {
                if (loginBonuses.IsSuccessStatusCode)
                {
                    Thread.Sleep(3000);
                    var httpResponse = await SAPI.SAPIGet("https://elb.seeddao.org/api/v1/streak-reward");
                    if (httpResponse is not null)
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                            var responseJson = await JsonSerializer.DeserializeAsync<SeedStreakRewardResponse>(responseStream);
                            return responseJson;
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> SeedClaimDailyReward(List<string> streakRewardIds)
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var request = new SeedStreakRewardRequest() { StreakRewardIds = streakRewardIds };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await SAPI.SAPIPost("https://elb.seeddao.org/api/v1/streak-reward", serializedRequestContent);
            if (httpResponse is not null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> SeedClaimSeed()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIPost("https://elb.seeddao.org/api/v1/seed/claim", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<SeedWormsResponse?> SeedWorms()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIGet("https://elb.seeddao.org/api/v1/worms");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<SeedWormsResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> SeedClaimWorm()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIPost("https://elb.seeddao.org/api/v1/worms/catch", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<bool> SeedClaimEgg()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIPost("https://elb.seeddao.org/api/v1/give-first-egg", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<SeedTasksResponse?> SeedTasks()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIGet("https://elb.seeddao.org/api/v1/tasks/progresses");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<SeedTasksResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> SeedDoneTask(string taskId)
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIPost($"https://elb.seeddao.org/api/v1/tasks/{taskId}", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<SeedGuildResponse?> SeedGuild()
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var httpResponse = await SAPI.SAPIGet("https://alb.seeddao.org/api/v1/guild/member/detail");
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<SeedGuildResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> SeedGuildLeave(string guildId)
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var request = new SeedGuildRequest() { GuildId = guildId };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await SAPI.SAPIPost("https://alb.seeddao.org/api/v1/guild/leave", serializedRequestContent);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }

        public async Task<bool> SeedGuildJoin(string guildId)
        {
            var SAPI = new SeedApi(PubQuery.Auth, PubQuery.Index, PubProxy);
            var request = new SeedGuildRequest() { GuildId = guildId };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var httpResponse = await SAPI.SAPIPost("https://alb.seeddao.org/api/v1/guild/join", serializedRequestContent);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;

            return false;
        }
    }
}