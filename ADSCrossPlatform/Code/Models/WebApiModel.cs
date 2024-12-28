using ADS.Code.Export;
using ADS.Code.Login;
using ADS.Code.Models;
using ADSCrossPlatform.Code.Login;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ADSCrossPlatform.Code.Models
{
    public class WebApiModel
    {
        // Вынесем базовые URL в константы для удобства
        private const string BaseUrl = "https://valyashki.ru/api";
        //private const string BaseUrl = "https://localhost:44356/api";
        //private const string LocalApiUrls = "https://localhost:44353/api";

        private StoredSettings _storedSettings;

        private readonly HttpClient _httpClient;

        public WebApiModel(StoredSettings storedSettings)
        {
            _httpClient = new HttpClient();
            _storedSettings = storedSettings;
        }

        public async Task<Dictionary<string, string>?> GetAllContentBySearchAsync(string searchData)
        {
            searchData = string.IsNullOrWhiteSpace(searchData) ? string.Empty : searchData;
            Dictionary<string, string> products = new Dictionary<string, string>();

            try
            {
                string url = $"{BaseUrl}/Order/adssearch?searchValue={Uri.EscapeDataString(searchData)}";

                string[] parts = searchData.Split('-');

                if (searchData.Split('-').Length == 4)
                {
                    url = $"{BaseUrl}/Addresses/searchplace?searchString={Uri.EscapeDataString(searchData)}";
                }

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                string data = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(data))
                {
                    try
                    {
                        var deserializedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                        if (deserializedData != null)
                        {
                            products = deserializedData;
                        }
                        else
                        {
                            LogException(new Exception("Deserialization returned null."));
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        LogException(jsonEx);
                    }
                }

                return products;
            }
            catch (HttpRequestException ex)
            {
                LogException(ex);
                return products;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return products;
            }
        }

        public async Task<ProductData> GetDataByArticle(string id, string[] storages)
        {
            ProductData product = new();
            try
            {
                string url = $"{BaseUrl}/Order/adswithid";
                var searchAndStorages = new SearchAndStorages
                {
                    SearchData = id,
                    Storages = storages
                };

                // Сериализация данных
                string json = JsonConvert.SerializeObject(searchAndStorages);
                StringContent stringContent = new(json, Encoding.UTF8, "application/json");

                // Выполнение запроса
                HttpResponseMessage response = await _httpClient.PostAsync(url, stringContent);

                // Проверка статуса ответа
                response.EnsureSuccessStatusCode();

                // Чтение ответа
                string rString = await response.Content.ReadAsStringAsync();

                // Проверка на пустоту строки ответа
                if (!string.IsNullOrEmpty(rString))
                {
                    // Попытка десериализации данных
                    var deserializedData = JsonConvert.DeserializeObject<ProductData>(rString);

                    if (deserializedData != null)
                    {
                        product = deserializedData;
                    }
                    else
                    {
                        // Логируем ошибку, если десериализация вернула null
                        LogException(new Exception("Deserialization returned null."));
                    }
                }
                else
                {
                    // Логируем случай, если ответ пуст
                    LogException(new Exception("Response string is empty."));
                }
            }
            catch (HttpRequestException ex)
            {
                // Ловим исключения, связанные с HTTP запросами
                LogException(ex);
            }
            catch (JsonException ex)
            {
                // Ловим исключения при десериализации
                LogException(ex);
            }
            catch (Exception ex)
            {
                // Ловим все остальные исключения
                LogException(ex);
            }

            return product;
        }

        public async Task<bool> CreateMoveWithArticles(
            List<ProductForMove> productForMove, 
            string toStore, 
            string fromStore, 
            string creatorName)
        {
            ProductsWithStores productsWithStores = new ProductsWithStores()
            {
                ProductsForMove = productForMove,
                stores = new string[2]
                {
                    toStore,
                    fromStore,
                },
                CreatorName = creatorName
            };

            try
            {
                string url = $"{BaseUrl}/Order/moveads";
                StringContent content = new StringContent(JsonConvert.SerializeObject(productsWithStores), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        public async Task<IEnumerable<AddressDBModel>?> GetContentByArticle(string article)
        {
            try
            {
                string url = $"{BaseUrl}/Addresses?article={Uri.EscapeDataString(article)}";
                string json = await _httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<IEnumerable<AddressDBModel>>(json);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        public async Task<IEnumerable<History>?> GetHistoryByArticle(string article)
        {
            try
            {
                string url = $"{BaseUrl}/Addresses/history?article={Uri.EscapeDataString(article)}";
                string json = await _httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<IEnumerable<History>>(json);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        public async Task<bool> PutAddressDBModel(Guid id, AddressDBModel addressDBModel)
        {
            try
            {
                AddressHistory addressHistory = new AddressHistory()
                {
                    addressDBModel = addressDBModel,
                    ChangedBy = _storedSettings.Username
                };

                string url = $"{BaseUrl}/Addresses/{id}";
                StringContent content = new StringContent(JsonConvert.SerializeObject(addressHistory), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        public async Task<bool> PostAddressDBModel(AddressDBModel addressDBModel)
        {
            try
            {
                AddressHistory addressHistory = new AddressHistory()
                {
                    addressDBModel = addressDBModel,
                    ChangedBy = _storedSettings.Username
                };
                addressHistory.addressDBModel.RowVersion = new byte[8];

                StringContent content = new StringContent(JsonConvert.SerializeObject(addressHistory), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/Addresses/create", content);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAddressDBModel(Guid id)
        {
            try
            {
                var json = JsonConvert.SerializeObject($"{_storedSettings.Username}");
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = $"{BaseUrl}/Addresses/{id}?changedBy={Uri.EscapeDataString(_storedSettings.Username)}";
                HttpResponseMessage response = await _httpClient.DeleteAsync(url);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        private void LogException(Exception ex)
        {
            Logger.LogException(ex);
        }

        public async Task<HttpResponseMessage> CheckData(LoginData loginData)
        {
            string url = $"{BaseUrl}/Order/check";
            string jsonData = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                return response;
            }
            catch (Exception ex)
            {
                // Возвращаем сообщение об ошибке с соответствующим статус-кодом
                var errorResponse = new
                {
                    Message = "Сервер не отвечает"
                };

                // Серилизуем ошибку в JSON
                string errorJson = JsonConvert.SerializeObject(errorResponse);
                var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(errorJson, Encoding.UTF8, "application/json")
                };

                return httpResponse;
            }
        }


        public async Task<LoginData?> Login(AccountData accountData)
        {
            try
            {
                string url = $"{BaseUrl}/Order/loginwithdata";

                string jsonData = JsonConvert.SerializeObject(accountData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                // return result != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(result) : new Dictionary<string, string>();
                var deserialized = JsonConvert.DeserializeObject<LoginData>(result);
                if (deserialized == null || string.IsNullOrWhiteSpace(deserialized.UserName)) return null;
                else return deserialized;
            }
            catch (HttpRequestException httpRequestException)
            {
                LogException(httpRequestException);
                return null;
            }
        }

        public async Task<LoginData?> LoginWithMemoID(AccountDataWithMemo accountDataWithMemo)
        {
            try
            {
                string url = $"{BaseUrl}/Order/getusertoken";

                string jsonData = JsonConvert.SerializeObject(accountDataWithMemo);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                // return result != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(result) : new Dictionary<string, string>();
                var deserialized = JsonConvert.DeserializeObject<LoginData>(result);
                if (deserialized == null || string.IsNullOrWhiteSpace(deserialized.UserName)) return null;
                else return deserialized;
            }
            catch (HttpRequestException httpRequestException)
            {
                LogException(httpRequestException);
                return null;
            }
        }

        public async Task<List<PrenoteModel>> GetPrenote(string storeId)
        {
            string url = $"{BaseUrl}/Addresses/prenote/{storeId}";

            string json = await _httpClient.GetStringAsync(url);
            List<PrenoteModel> data = 
                JsonConvert.DeserializeObject<List<PrenoteModel>>(json) ?? 
                new List<PrenoteModel>();

            return data;
        }

        public async Task<SG010Report> GetSG010()
        {
            string baseUrl = $"{BaseUrl}/AdsPlaces";
            int page = 1, pageSize = 10;

            SG010Report result = new SG010Report();

            do
            {
                // Запрос текущей страницы
                var response = await _httpClient.GetFromJsonAsync<SG010Report>(baseUrl + $"?page={page}&pageSize={pageSize}");

                if (response == null) break;

                result.Data.AddRange(response.Data);

                // Проверяем наличие следующей страницы
                if (!response.HasNextPage)
                    break;

                page++;

            } while (true);

            return result;
        }

        public async Task<List<Allocation>> GetAllocation()
        {
            string url = $"{BaseUrl}/Allocation/allocation";
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                List<Allocation> data =
                    JsonConvert.DeserializeObject<List<Allocation>>(json) ??
                    new List<Allocation>();

                return data;
            }
            catch (Exception)
            {

                return new List<Allocation> { };
            }

        }
    }
}


