using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VNGod.Network.VNDB
{
    static class Api
    {
        private const string BaseUrl = "https://api.vndb.org/kana";
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        private static string GetToken()
        {
            return Properties.Settings.Default.VNDBToken ?? string.Empty;
        }
        public static async Task<Result> PostSearchAsync(string keyword)
        {
            var client = new HttpClient();
            InitializeClient(client);
            var searchModel = new SearchModel
            {
                filters = ["search","=",keyword],
                fields = "titles.lang,titles.title"
            };
            var json = JsonSerializer.Serialize(searchModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(BaseUrl + "/vn", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SearchResult>(responseBody, jsonOptions);
            return result?.results.FirstOrDefault() ?? throw new Exception("No results found.");
        }
        public static async Task<Result> PostGetNameAsync(string id)
        {
            var client = new HttpClient();
            InitializeClient(client);
            var searchModel = new SearchModel
            {
                filters = ["id","=",id],
                fields = "titles.lang,titles.title"
            };
            var json = JsonSerializer.Serialize(searchModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(BaseUrl + "/vn", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<SearchResult>(responseBody, jsonOptions);
            return result?.results.FirstOrDefault() ?? throw new Exception("No results found.");
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
