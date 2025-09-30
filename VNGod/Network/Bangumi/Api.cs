using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace VNGod.Network.Bangumi
{
    static class Api
    {
        private const string BaseUrl = "https://api.bgm.tv";
        /// <summary>
        /// Gets the stored Bangumi token from application settings.
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            return Properties.Settings.Default.BgmToken ?? string.Empty;
        }
        public static async Task<Datum> PostSearchAsync(string keyword)
        {
            var client = new HttpClient();
            InitializeClient(client);
            var searchModel = new SearchModel
            {
                keyword = keyword,
                filter = new Filter
                {
                    type = [4]
                }
            };
            var json = System.Text.Json.JsonSerializer.Serialize(searchModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(BaseUrl + "/v0/search/subjects", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = System.Text.Json.JsonSerializer.Deserialize<SearchResult>(responseBody, options);
            return result?.data.FirstOrDefault() ?? throw new Exception("No results found.");
        }

        private static void InitializeClient(HttpClient client)
        {
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SamHou0-VNGod", "0.0.1"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(https://github.com/SamHou0/VNGod)"));
            if (!string.IsNullOrEmpty(GetToken()))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
        }
    }
}
