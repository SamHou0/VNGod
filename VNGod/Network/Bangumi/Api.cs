using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VNGod.Network.Bangumi
{
    static class Api
    {
        private const string BaseUrl = "https://api.bgm.tv";
        public static async Task<Datum> PostSearchAsync(string keyword)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "VNGod");
            var searchModel = new SearchModel
            {
                keyword = keyword,
                filter = new Filter
                {
                    type = [4],
                    nsfw = false
                }
            };
            var json = System.Text.Json.JsonSerializer.Serialize(searchModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(BaseUrl+"/v0/search/subjects", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var result = System.Text.Json.JsonSerializer.Deserialize<SearchResult>(responseBody, options);
            return result?.data.FirstOrDefault() ?? throw new Exception("No results found.");
        }
        
    }
}
