using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ADSCrossPlatform.Code.Models
{
    public class AddressViewModel : INotifyPropertyChanged
    {
        private AddressModel? _selectedAddressModel;
        private readonly DataManager _dataManager;

        private string _article = string.Empty;

        public string ArticleForNewAddress { get; set; }

        public string Article
        {
            get => _article;
            set
            {
                if (_article != value)
                {
                    _article = value;
                    OnPropertyChanged();
                    // Используем async void, чтобы безопасно вызывать await внутри сеттера
                    async void LoadData() => await LoadAddressesAsync();
                    LoadData();
                }
            }
        }

        private ObservableCollection<AddressModel> _addresses = new();
        public ObservableCollection<AddressModel> Addresses
        {
            get => _addresses;
            private set
            {
                _addresses = value;
                OnPropertyChanged();
            }
        }

        // Индикация загрузки данных
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        // Конструктор без article
        public AddressViewModel(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        // Асинхронный метод для загрузки данных по артикулу
        public async Task LoadAddressesAsync()
        {
            if (string.IsNullOrWhiteSpace(Article))
            {
                Addresses.Clear();  // Очищаем коллекцию, если артикул пустой
                return;
            }

            IsLoading = true;

            var result = await _dataManager.GetContentByArticleAsync(Article);

            if (result != null)
            {
                Addresses = new ObservableCollection<AddressModel>(result);
            }
            else
            {
                Addresses.Clear();  // Очищаем коллекцию, если результат null
            }
            await Task.Delay(2000);

            IsLoading = false;
        }

        internal AddressModel? SelectedAddressModel
        {
            get => _selectedAddressModel;
            set
            {
                _selectedAddressModel = value;
                //OnPropertyChanged(nameof(SelectedAddressModel));
            }
        }

        public AddressDBModel AddressDBModel => _dataManager.AddressDBModel;

        // Создание новой записи
        public async Task<bool> CreateRecord(AddressDBModel addressDBModel)
        {
            bool result = await _dataManager.CreateRecordAsync(addressDBModel);
            await LoadAddressesAsync();  // Обновляем список после создания
            return result;
        }

        // Редактирование записи
        public async Task<bool> EditRecord(AddressDBModel addressDBModel)
        {
            bool result = await _dataManager.EditRecordAsync(addressDBModel);
            await LoadAddressesAsync();  // Обновляем список после редактирования
            return result;
        }

        // Удаление записи
        public async Task<bool> DeleteRecord(AddressDBModel addressDBModel)
        {
            bool result = await _dataManager.DeleteRecordAsync(addressDBModel);
            await LoadAddressesAsync();  // Обновляем список после удаления
            return result;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        // Уведомление об изменениях свойств
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
