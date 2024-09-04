using ADSCrossPlatform.Code.Models;
using System.Collections.ObjectModel;

namespace ADSCrossPlatform;

public partial class MainPageAndroid : ContentPage
{
    private readonly DataManager _dataManager;
    private readonly AddressViewModel _addressViewModel;
    private readonly StoredSettings _storedSettings;

    private Dictionary<string, string> _searchResultsDictionary;
    private ObservableCollection<string> _searchResultValues;
    private Dictionary<string, string> _storages;

    public MainPageAndroid(DataManager dataManager, AddressViewModel addressViewModel, StoredSettings storedSettings)
    {
        InitializeComponent();
        windowPicker.SelectedIndex = 2;

        _dataManager = dataManager;
        _addressViewModel = addressViewModel;
        _storedSettings = storedSettings;

        // Инициализация полей
        _searchResultsDictionary = new Dictionary<string, string>();
        _searchResultValues = new ObservableCollection<string>();
        BindingContext = _addressViewModel;
        SearchResultsListView.ItemsSource = _searchResultValues;

        _storages = new Dictionary<string, string>();
    }

    public void SetStorages(Dictionary<string, string> storages)
    {
        _storages = storages;
        //2117cbc2-9f30-11ee-0a80-026e0010b8bf - TT390
        //8cbcce7c-aede-11ee-0a80-0dfd007bd0bc - TT310
        //8eecadfd-0367-11ed-0a80-0b430036e29c - ЧернаяРечка
    }

    public void InitSettings()
    {
        if (_storedSettings == null) return;
        _storedSettings.MoveToStore310 = _storedSettings.MoveToStore310 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "ТТ310").Key : _storedSettings.MoveToStore310;
        _storedSettings.MoveFromStore310 = _storedSettings.MoveFromStore310 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "ЧернаяРечка").Key : _storedSettings.MoveFromStore310;
        _storedSettings.MoveToStore390 = _storedSettings.MoveToStore390 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "ТТ390").Key : _storedSettings.MoveToStore390;
        _storedSettings.MoveFromStore390 = _storedSettings.MoveFromStore390 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "ЧернаяРечка").Key : _storedSettings.MoveFromStore390;

    }

    public void AdminLogged()
    {
        // Настройки для админов
    }

    private bool IsBlocked()
    {
        return (LoginActivityIndicator.IsRunning || LoadActivityIndicator.IsRunning);
    }

    // Обработчик нажатия на кнопку "Поиск"
    private async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;
        string query = SearchField.Text;


        if (string.IsNullOrWhiteSpace(query))
        {
            await DisplayAlert("Ошибка", "Введите артикул для поиска", "OK");
            return;
        }

        await PerformSearch(query);
    }

    private void IndicatorSetActive(bool value)
    {
        LoginActivityIndicator.IsVisible = value;
        LoginActivityIndicator.IsRunning = value;
    }

    private async Task PerformSearch(string query)
    {
        try
        {
            IndicatorSetActive(true);
            _searchResultsDictionary = await _dataManager.GetDataBySearchAsync(query);
            IndicatorSetActive(false);
            _searchResultValues.Clear();
            foreach (var resultValue in _searchResultsDictionary.Values)
            {
                _searchResultValues.Add(resultValue);
            }
            //await Task.Delay(100);
            //SearchResultsListView.IsVisible = _searchResultValues.Count > 0;

        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось получить данные: {ex.Message}", "OK");
        }
    }

    private async void ToNewPage()
    {
        var newRecord = new AddressDBModel { Article = ArticleField.Text };
        _addressViewModel.SelectedAddressModel = new AddressModel(new AddressDBModel())
        {
            Article = ArticleField.Text
        };

        var New_address_Page = App.ServiceProvider.GetRequiredService<NewAddressPage>();
        await Navigation.PushAsync(New_address_Page);
    }

    private AddressDBModel Convert(AddressModel addressModel)
    {
        return new AddressDBModel
        {
            Id = addressModel.Id,
            Article = addressModel.Article,
            Zone = addressModel.Zone,
            Row = addressModel.Row,
            Place = addressModel.Place,
            Level = addressModel.Level,
            Qty = addressModel.Qty
        };
    }

    private void Create_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(SearchField.Text))
        {
            ToNewPage();
        }
    }

    private async void PickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsBlocked())
        {
            windowPicker.SelectedIndex = 2;
            return;
        }
        if (windowPicker.SelectedItem.ToString() == "310")
        {
            var mainPage = App.ServiceProvider.GetRequiredService<Window_310>();
            await Navigation.PushAsync(mainPage);
        }

        if (windowPicker.SelectedItem.ToString() == "390")
        {
            var mainPage = App.ServiceProvider.GetRequiredService<Window_390>();
            await Navigation.PushAsync(mainPage);
        }

        if (windowPicker.SelectedItem.ToString() == "450")
        {
            var Widnow_310_Back_Page = App.ServiceProvider.GetRequiredService<Window_310_Back>();
            await Navigation.PushAsync(Widnow_310_Back_Page);
        }
    }

    private async void SearchResultsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (IsBlocked()) return;
        if (e.Item is string selectedValue)
        {
            var correspondingKey = _searchResultsDictionary.FirstOrDefault(i => i.Value == selectedValue).Key;
            IndicatorSetActive(true);
            var productData = await _dataManager.GetDataByIDAsync(correspondingKey, _storages.Values.ToArray());
            IndicatorSetActive(false);
            ArticleField.Text = productData.Article;
            ArtNameField.Text = productData.Name;
            ImageViewer.Source = productData.ImageUrl;
            
            //_addressViewModel.Addresses.Clear();
            _addressViewModel.Article = productData.Article;

            ArtQtyField.Text = productData?.QtyInStorages?.Sum().ToString();


            _searchResultValues.Clear();
            SearchResultsListView.ItemsSource = _searchResultValues;
            SearchResultsListView.SelectedItem = null;
            SearchField.Text = string.Empty;
        }
    }

    private async void DataListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (IsBlocked()) return;
        if (e.Item is AddressModel selectedAddress)
        {
            _addressViewModel.SelectedAddressModel = selectedAddress;
            var Details_Page = App.ServiceProvider.GetRequiredService<Details>();
            await Navigation.PushAsync(Details_Page);
        }
    }
}
