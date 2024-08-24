﻿using ADSCrossPlatform.Code.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ADSCrossPlatform.Code.Login;

namespace ADSCrossPlatform.Code.Models
{
    public class WebApiModel
    {
        // Вынесем базовые URL в константы для удобства
        private const string BaseUrl = "https://valyashki.ru/api";
        private const string LocalApiUrls = "https://localhost:44353/api";

        private readonly HttpClient _httpClient;

        public WebApiModel()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Dictionary<string, string>?> GetAllContentBySearchAsync(string searchData)
        {
            searchData = string.IsNullOrWhiteSpace(searchData) ? string.Empty : searchData;
            Dictionary<string, string> products = new Dictionary<string, string>();

            try
            {
                string url = $"{BaseUrl}/Order/adssearch?searchValue={Uri.EscapeDataString(searchData)}";
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


        //public async Task<IEnumerable<AddressDBModel>?> GetAllContentAsync()
        //{
        //    try
        //    {
        //        string url = $"{BaseUrl}/Addresses";
        //        string json = await _httpClient.GetStringAsync(url);
        //        return JsonConvert.DeserializeObject<IEnumerable<AddressDBModel>>(json);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException(ex);
        //        return null;
        //    }
        //}

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

        //public async Task<string> GetDataFromMyWarehouseByArt(string article)
        //{
        //    string url = $"https://api.moysklad.ru/api/remap/1.2/entity/assortment?filter=article~{Uri.EscapeDataString(article)}";
        //    return await GetDataFromMyWarehouse(url);
        //}

        //public async Task<string> GetDataFromMyWarehouseByName(string article)
        //{
        //    string url = $"https://api.moysklad.ru/api/remap/1.2/entity/assortment?filter=name~{Uri.EscapeDataString(article)}";
        //    return await GetDataFromMyWarehouse(url);
        //}

        //private async Task<string> GetDataFromMyWarehouse(string targetUrl)
        //{
        //    using var handler = new HttpClientHandler
        //    {
        //        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        //    };

        //    using var client = new HttpClient(handler);

        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "10ee53a08a7ef85e882e9aab0721cd983f430bb5");
        //    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        //    try
        //    {
        //        HttpResponseMessage response = await client.GetAsync(targetUrl);
        //        response.EnsureSuccessStatusCode();
        //        return await response.Content.ReadAsStringAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException(ex);
        //        return string.Empty;
        //    }
        //}

        public async Task<bool> PutAddressDBModel(Guid id, AddressDBModel addressDBModel)
        {
            try
            {
                string url = $"{BaseUrl}/Addresses/{id}";
                StringContent content = new StringContent(JsonConvert.SerializeObject(addressDBModel), Encoding.UTF8, "application/json");

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
                StringContent content = new StringContent(JsonConvert.SerializeObject(addressDBModel), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/Addresses", content);
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
                string url = $"{BaseUrl}/Addresses/{id}";
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

        //private List<ADSProductObserve> Convert(List<ProductAdaptedForADS> productsAdapted)
        //{
        //    return productsAdapted?.Select(item => new ADSProductObserve(item)).ToList() ?? new List<ADSProductObserve>();
        //}

        internal async Task<Dictionary<string, string>?> Login(AccountData accountData)
        {
            try
            {
                string url = $"{BaseUrl}/Order/login";

                string jsonData = JsonConvert.SerializeObject(accountData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();
                // return result != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(result) : new Dictionary<string, string>();
                var deserialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                if (deserialized == null || deserialized.Count <= 0) return null;
                else return deserialized;
            }
            catch (HttpRequestException httpRequestException)
            {
                LogException(httpRequestException);
                return null;
            }
        }
    }
}


