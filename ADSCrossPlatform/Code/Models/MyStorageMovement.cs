using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ADSCrossPlatform.Code.Models
{
    public class MyStorageMovement
    {
        public async Task<HttpResponseMessage> AddPositionToMevement(string articleId, int articleQty)
        {
            // Создаю перемещение в моем складе
            string movementId = await CreateMovement();

            string url = $"https://api.moysklad.ru/api/remap/1.2/entity/move/{movementId}/positions";
            var item = new Item
            {
                Quantity = articleQty,
                Assortment = new Assortment
                {
                    Meta = new Meta
                    {
                        Href = $"https://api.moysklad.ru/api/remap/1.2/entity/product/{articleId}",
                        MetadataHref = "https://api.moysklad.ru/api/remap/1.2/entity/product/metadata",
                        Type = "product",
                        MediaType = "application/json"
                    }
                },
                Overhead = 0
            };
            string position = JsonConvert.SerializeObject(item);

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient client = new(handler);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "10ee53a08a7ef85e882e9aab0721cd983f430bb5");
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var data = new StringContent(position, Encoding.UTF8, "application/json");
            var r = await client.PostAsync(url, data);
            return r;
        }

        private async Task<string> CreateMovement()
        {
            string url = "https://api.moysklad.ru/api/remap/1.2/entity/move";

            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            HttpClient client = new(handler);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "10ee53a08a7ef85e882e9aab0721cd983f430bb5");
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            string json = await GetJsonRequestBody();
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var r = client.PostAsync(url, data).Result;
            string content = r.Content.ReadAsStringAsync().Result;
            dynamic? jsonObject = JsonConvert.DeserializeObject(content);

            dynamic? id;
            try
            {
                id = jsonObject?.id;
            }
            catch (Exception)
            {
                id = "-1";
            }
            return id;
        }

        private async Task<string> GetJsonRequestBody()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("CreateMovementJson.txt");
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return "";
            }
        }




        public class Assortment
        {
            [JsonProperty("meta")]
            public Meta Meta { get; set; }
        }

        public class Meta
        {
            [JsonProperty("href")]
            public string Href { get; set; }

            [JsonProperty("metadataHref")]
            public string MetadataHref { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("mediaType")]
            public string MediaType { get; set; }
        }

        public class Item
        {
            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            //[JsonProperty("price")]
            //public double Price { get; set; }

            [JsonProperty("assortment")]
            public Assortment Assortment { get; set; }

            [JsonProperty("overhead")]
            public int Overhead { get; set; }
        }

    }
}
