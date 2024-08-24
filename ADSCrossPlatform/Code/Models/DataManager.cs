using ADSCrossPlatform.Code.Interfaces;
using ADSCrossPlatform.Code.Login;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;

namespace ADSCrossPlatform.Code.Models
{
    public class DataManager
    {
        private readonly WebApiModel _dataBaseApiModel;

        public DataManager(WebApiModel webApiModel)
        {
            _dataBaseApiModel = webApiModel;
        }

        // Возвращаем новый экземпляр модели адреса
        public AddressDBModel AddressDBModel => new AddressDBModel();

        // Получение данных по ID и складам
        public async Task<ProductData> GetDataByIDAsync(string id, string[] storages)
        {
            return await _dataBaseApiModel.GetDataByArticle(id, storages);
        }

        // Получение контента по артикулу
        public async Task<IEnumerable<AddressModel>?> GetContentByArticleAsync(string article)
        {
            var content = await _dataBaseApiModel.GetContentByArticle(article);
            return content != null ? Convert<AddressModel>(content) : null;
        }

        // Поиск данных по запросу
        public async Task<Dictionary<string, string>> GetDataBySearchAsync(string searchData)
        {
            return await _dataBaseApiModel.GetAllContentBySearchAsync(searchData) ?? new Dictionary<string, string>();
        }

        //// Получение всей информации по артикулу или названию
        //public async Task<string[]> GetAllInfoAsync(string article, bool isFindByArticle)
        //{
        //    string[] allInfo = new string[5];
        //    string jsonString = isFindByArticle
        //        ? await _dataBaseApiModel.GetDataFromMyWarehouseByArt(article)
        //        : await _dataBaseApiModel.GetDataFromMyWarehouseByName(article);

        //    dynamic? jsonObject = JsonConvert.DeserializeObject(jsonString);

        //    if (jsonObject == null || jsonObject.rows == null || jsonObject.rows.Count == 0)
        //    {
        //        return allInfo;
        //    }

        //    try
        //    {
        //        allInfo[0] = jsonObject.rows[0].name ?? string.Empty;
        //        allInfo[1] = jsonObject.rows[0].stock?.ToString() ?? string.Empty;
        //        allInfo[2] = jsonObject.rows[0].id ?? string.Empty;
        //        allInfo[3] = (jsonObject.rows[0].salePrices[0]?.value ?? string.Empty).ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Логируем исключение при парсинге JSON
        //        Logger.LogException(ex);
        //    }

        //    return allInfo;
        //}

        // Создание записи
        public async Task<bool> CreateRecordAsync(AddressDBModel addressDBModel)
        {
            return await _dataBaseApiModel.PostAddressDBModel(addressDBModel);
        }

        // Редактирование записи
        public async Task<bool> EditRecordAsync(AddressDBModel addressDBModel)
        {
            return await _dataBaseApiModel.PutAddressDBModel(addressDBModel.Id, addressDBModel);
        }

        // Удаление записи
        public async Task<bool> DeleteRecordAsync(AddressDBModel addressDBModel)
        {
            return await _dataBaseApiModel.DeleteAddressDBModel(addressDBModel.Id);
        }

        // Преобразование исходной коллекции в ObservableCollection<T>
        private ObservableCollection<T> Convert<T>(IEnumerable original) where T : class
        {
            var converted = original.OfType<AddressDBModel>()
                                    .Select(dbModel => new AddressModel(dbModel))
                                    .OfType<T>()
                                    .ToList();

            return new ObservableCollection<T>(converted);
        }

        internal async Task<Dictionary<string, string>> Login(AccountData accountData)
        {
            return await _dataBaseApiModel.Login(accountData);
        }
    }
}



//using ADSCrossPlatform.Code.Interfaces;
//using Newtonsoft.Json;
//using System.Collections;
//using System.Collections.ObjectModel;

//namespace ADSCrossPlatform.Code.Models
//{
//    public class DataManager
//    {
//        private WebApiModel _dataBaseApiModel;
//        //public DataManager(DataBaseApiModel dataBaseApiModel) 
//        //{
//        //    _dataBaseApiModel = dataBaseApiModel;
//        //}

//        public DataManager(WebApiModel webApiModel)
//        {
//            _dataBaseApiModel = webApiModel;
//        }

//        public AddressDBModel addressDBModel
//        {
//            get
//            {
//                return new AddressDBModel();
//            }
//        }

//        //public IEnumerable<AddressDBModel>? GetAllDataSimple()
//        //{
//        //    return _dataBaseApiModel.GetAllContent();
//        //}

//        //public ObservableCollection<AddressModel> GetAllData()
//        //{
//        //    return Convert<AddressModel>(_dataBaseApiModel.GetAllContent());
//        //}

//        public async Task<ProductData> GetDataByID(string id, string[] storages)
//        {
//            return await _dataBaseApiModel.GetDataByArticle(id, storages);
//        }

//        public async Task<IEnumerable<AddressModel>?> GetContentByArticle(string article)
//        {
//            var temp = await _dataBaseApiModel.GetContentByArticle(article);
//            if (temp != null) return Convert<AddressModel>(temp);
//            else return null;
//        }

//        public async Task<Dictionary<string, string>> GetDataBySearch(string searchData)
//        {
//            var t =  await _dataBaseApiModel.GetAllContentBySearchAsync(searchData);
//            return t;
//        }

//        public string[] GetAllInfo(string article, bool isFindByArticle)
//        {
//            string[] allInfo = new string[5];
//            string jsonString;
//            if (isFindByArticle) jsonString = _dataBaseApiModel.GetDataFromMyWarehouseByArt(article).Result;
//            else jsonString = _dataBaseApiModel.GetDataFromMyWarehouseByName(article).Result;

//            dynamic? jsonObject = JsonConvert.DeserializeObject(jsonString);

//            dynamic? artName;
//            dynamic? artQty;
//            dynamic? id;
//            dynamic? price;
//            try
//            {
//                artName = jsonObject?.rows[0].name;
//                artQty = jsonObject?.rows[0].stock;
//                id = jsonObject?.rows[0].id;
//                price = jsonObject?.rows[0].salePrices[0].value;
//            }
//            catch (Exception)
//            {
//                allInfo[0] = string.Empty;
//                allInfo[1] = string.Empty;
//                allInfo[2] = string.Empty;
//                allInfo[3] = string.Empty;
//                return allInfo;
//            }

//            if (artName != null) allInfo[0] = artName;
//            if (artQty != null) allInfo[1] = artQty;
//            if (id != null) allInfo[2] = id;
//            if (price != null) allInfo[3] = price;
//            return allInfo;
//        }

//        public async Task<bool> CreateRecord(AddressDBModel addressDBModel)
//        {
//            return await _dataBaseApiModel.PostAddressDBModel(addressDBModel);
//        }

//        public async Task<bool> EditRecord(AddressDBModel addressDBModel)
//        {
//            return await _dataBaseApiModel.PutAddressDBModel(addressDBModel.Id, addressDBModel);
//        }

//        public async Task<bool> DeleteRecord(AddressDBModel addressDBModel)
//        {
//            return await _dataBaseApiModel.DeleteAddressDBModel(addressDBModel.Id);
//        }

//        private ObservableCollection<T> Convert<T>(IEnumerable original)
//        {
//            List<AddressModel> converted = new List<AddressModel>();
//            foreach (var e in original)
//            {
//                if (e != null) converted.Add(new AddressModel(e as AddressDBModel));
//            }
//            return new ObservableCollection<T>(converted.Cast<T>());
//        }
//    }
//}
