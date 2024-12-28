using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ADSCrossPlatform.Code.Models
{
    public class AddressViewModel : INotifyPropertyChanged
    {
        private readonly DataManager _dataManager;

        private string _article = string.Empty;
        private string _filterText = string.Empty;
        private bool _isLoading;
        private AddressModel? _selectedAddressModel;
        private ObservableCollection<AddressModel> _addresses = new();
        private ObservableCollection<AddressModel> _filteredAddresses = new();

        // Конструктор
        public AddressViewModel(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        #region Свойства

        public string Article
        {
            get => _article;
            set
            {
                if (_article != value)
                {
                    _article = value;
                    OnPropertyChanged();
                    LoadAddressesAsync(); // Асинхронная загрузка данных
                }
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged();
                    ApplyFilter(); // Применение фильтра
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<AddressModel> Addresses
        {
            get => _addresses;
            private set
            {
                _addresses = value;
                OnPropertyChanged();
                ApplyFilter(); // Обновление фильтра при изменении данных
            }
        }

        public ObservableCollection<AddressModel> FilteredAddresses
        {
            get => _filteredAddresses;
            private set
            {
                _filteredAddresses = value;
                OnPropertyChanged();
            }
        }

        public AddressModel? SelectedAddressModel
        {
            get => _selectedAddressModel;
            set
            {
                _selectedAddressModel = value;
                OnPropertyChanged();
            }
        }

        public AddressDBModel AddressDBModel => _dataManager.AddressDBModel;

        #endregion

        #region Методы

        // Загрузка данных по артикулу
        public async Task LoadAddressesAsync()
        {
            if (string.IsNullOrWhiteSpace(Article))
            {
                Addresses.Clear();
                return;
            }

            IsLoading = true;

            var result = await _dataManager.GetContentByArticleAsync(Article);

                Addresses = result != null
                ? new ObservableCollection<AddressModel>(result.OrderByDescending(a => a.IsSalesLocation))
                : new ObservableCollection<AddressModel>();
            ApplyFilter();

            IsLoading = false;
        }

        // Применение фильтра
        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterText) || FilterText == "Все")
            {
                FilteredAddresses = new ObservableCollection<AddressModel>(Addresses);
            }
            else
            {
                FilteredAddresses = new ObservableCollection<AddressModel>(
                    Addresses.Where(a => a.StoreID.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).
                    OrderByDescending(a => a.IsSalesLocation)
                );
            }
        }

        // Создание записи
        public async Task<bool> CreateRecord(AddressDBModel addressDBModel)
        {
            var result = await _dataManager.CreateRecordAsync(addressDBModel);
            if (result) await LoadAddressesAsync();
            return result;
        }

        // Редактирование записи
        public async Task<bool> EditRecord(AddressDBModel addressDBModel)
        {
            var result = await _dataManager.EditRecordAsync(addressDBModel);
            if (result) await LoadAddressesAsync();
            return result;
        }

        // Удаление записи
        public async Task<bool> DeleteRecord(AddressDBModel addressDBModel)
        {
            var result = await _dataManager.DeleteRecordAsync(addressDBModel);
            if (result) await LoadAddressesAsync();
            return result;
        }

        #endregion

        #region Реализация INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
