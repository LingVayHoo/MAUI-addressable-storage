using ADS.Code.Export;
using ADS.Code.Login;
using ADS.Code.Models;
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

        public async Task<IEnumerable<HistoryFormated>?> GetHistoryByArticleAsync(string article)
        {
            var content = await _dataBaseApiModel.GetHistoryByArticle(article);
            return content != null ? HistoryConvert(content) : null;
        }

        private ObservableCollection<HistoryFormated> HistoryConvert(IEnumerable original)
        {

            var converted = original.OfType<History>()
                                    .Select(history => new HistoryFormated(history))
                                    .OfType<HistoryFormated>()
                                    .ToList();

            return new ObservableCollection<HistoryFormated>(converted);
        }

        // Поиск данных по запросу
        public async Task<Dictionary<string, string>> GetDataBySearchAsync(string searchData)
        {
            return await _dataBaseApiModel.GetAllContentBySearchAsync(searchData) ?? new Dictionary<string, string>();
        }

        public async Task<bool> CreateMoveWithArticles(
            List<ProductForMove> productForMove, 
            string toStore, 
            string fromStore,
            string creatorName)
        {
            return await _dataBaseApiModel.CreateMoveWithArticles(productForMove, toStore, fromStore, creatorName);
        }

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

        public async Task<HttpResponseMessage> CheckData(LoginData loginData)
        {
            return await _dataBaseApiModel.CheckData(loginData);
        }

        internal async Task<LoginData?> Login(AccountData accountData)
        {
            return await _dataBaseApiModel.Login(accountData);
        }

        internal async Task<LoginData?> LoginWithMemo(AccountDataWithMemo loginWithMemo)
        {
            return await _dataBaseApiModel.LoginWithMemoID(loginWithMemo);
        }

        internal async Task<List<PrenoteModel>> GetPrenote(string storeId)
        {
            return await _dataBaseApiModel.GetPrenote(storeId);
        }

        internal async Task<SG010Report>GetSG010()
        {
            return await _dataBaseApiModel.GetSG010();
        }

        internal async Task<List<Allocation>> GetAllocation()
        {
            return await _dataBaseApiModel.GetAllocation();
        }
    }
}


