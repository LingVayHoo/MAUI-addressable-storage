//using ADSCrossPlatform.Code.Interfaces;
//using System.Net;
//using System.Net.Http.Headers;

//namespace ADSCrossPlatform.Code.Models
//{
//    public class PostgreSQLApiModel : IDataBase
//    {
//        private Postgre_SQL _postgreSQL;

//        public PostgreSQLApiModel()
//        {
//            _postgreSQL = new();
//        }
//        public async Task<IEnumerable<AddressDBModel>?> GetAllContentAsync()
//        {
//            var res = await _postgreSQL.GetAddresses();
//            return res;
//        }

//        public IEnumerable<AddressDBModel>? GetContentByArticle(string article)
//        {
//            var all = GetAllContent();
//            List<AddressDBModel> result = [];
//            foreach (var e in all)
//            {
//                if (e.Article == article)
//                    result.Add(e);
//            }
//            return result;
//        }

//        public async Task<string> GetDataFromMyWarehouseByArt(string article)
//        {
//            string url = $"https://api.moysklad.ru/api/remap/1.2/entity/assortment?filter=article~{article}";
//            return await GetDataFromMyWarehouse(article, url);
//        }

//        public async Task<string> GetDataFromMyWarehouseByName(string article)
//        {
//            string url = $"https://api.moysklad.ru/api/remap/1.2/entity/assortment?filter=name~{article}";
//            return await GetDataFromMyWarehouse(article, url);
//        }

//        public async Task<string> GetDataFromMyWarehouse(string article, string targetUrl)
//        {
//            HttpClientHandler handler = new HttpClientHandler()
//            {
//                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
//            };
//            HttpClient client = new(handler);

//            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "32811361d1b7369778cf699223fe8cccd16bcfb6");
//            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
//            string url = targetUrl;
//            var r = client.GetAsync(url).Result;
//            string content = r.Content.ReadAsStringAsync().Result;
//            //var json = JObject.Parse(content);
//            return content;
//        }

//        public IEnumerable<AddressDBModel>? GetAllContent()
//        {
//            var r = Task.Run(() => GetAllContentAsync()).Result;
//            return r;
//        }

//        public async Task<bool> PutAddressDBModel(Guid id, AddressDBModel addressDBModel)
//        {
//            return await _postgreSQL.PutAddressDBModel(id, addressDBModel);

//        }

//        public async Task<bool> PostAddressDBModel(AddressDBModel addressDBModel)
//        {
//            return await _postgreSQL.PostAddressDBModel(addressDBModel);
//        }

//        public async Task<bool> DeleteAddressDBModel(Guid id)
//        {
//            return await _postgreSQL.DeleteAddressDBModel(id);
//        }
//    }
//}
