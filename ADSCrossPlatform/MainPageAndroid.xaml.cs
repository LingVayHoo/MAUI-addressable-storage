using ADSCrossPlatform.Code.Models;
using System.Collections.ObjectModel;

namespace ADSCrossPlatform;

public partial class MainPageAndroid : ContentPage
{
    private DataManager _dataManager;
    private AddressViewModel _addressViewModel;

    private Dictionary<string, string> _searchResultsDictionary;
    private ObservableCollection<string> _searchResultValues;
    private Dictionary<string, string> _storages;

    public MainPageAndroid()
    {
        InitializeComponent();
        windowPicker.SelectedIndex = 1;
        _dataManager = new DataManager(new WebApiModel());
        _addressViewModel = new AddressViewModel(_dataManager);

        // Инициализация полей
        _searchResultsDictionary = new Dictionary<string, string>();
        _searchResultValues = new ObservableCollection<string>();
        SearchResultsListView.ItemsSource = _searchResultValues;

        _storages = new Dictionary<string, string>();

        BindingContext = _addressViewModel;
    }

    public void SetStorages(Dictionary<string, string> storages)
    {
        _storages = storages;
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

    private async void ToDetailsPage(AddressModel selectedAddress)
    {
        
        var addressDBModel = Convert(selectedAddress);
        await Navigation.PushAsync(new Details(_addressViewModel, addressDBModel, false));
    }

    private async void ToNewPage()
    {
        var newRecord = new AddressDBModel { Article = ArticleField.Text };
        await Navigation.PushAsync(new Details(_addressViewModel, newRecord, true));
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
            windowPicker.SelectedIndex = 1;
            return;
        }
        if (windowPicker.SelectedItem.ToString() == "31")
        {
            await Navigation.PushAsync(new Window_310());
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
            //SearchResultsListView.IsVisible = false;
            SearchResultsListView.SelectedItem = null;
        }
    }

    private void DataListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (IsBlocked()) return;
        if (e.Item is AddressModel selectedAddress)
        {
            ToDetailsPage(selectedAddress);
        }
    }
}
