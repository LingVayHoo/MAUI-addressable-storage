namespace ADSCrossPlatform;
using ADSCrossPlatform.Code.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

public partial class Window_310_Back : ContentPage
{
    private DataManager _dataManager;
    private AddressViewModel _addressViewModel;
    private StoredSettings _storedSettings;
    private ProductData _productData;

    private Dictionary<string, string> _searchResultsDictionary;
    private ObservableCollection<string> _searchResultValues;
    private Dictionary<string, string> _storages;

    private ProductData _selectedProduct;

    private int qtyInStore;

    public Window_310_Back(DataManager dataManager, AddressViewModel addressViewModel, StoredSettings storedSettings)
    {
        InitializeComponent();

        //Внедрение зависимостей
        _dataManager = dataManager;
        _addressViewModel = addressViewModel;
        _storedSettings = storedSettings;

        // Инициализация полей
        _searchResultsDictionary = new Dictionary<string, string>();
        _searchResultValues = new ObservableCollection<string>();
        _storages = new Dictionary<string, string>();
        _productData = new ProductData();
        _selectedProduct = new ProductData();

        SearchResultsListView.ItemsSource = _searchResultValues;

        windowPicker.SelectedIndex = 3;
    }

    private async void PickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsBlocked())
        {
            windowPicker.SelectedIndex = 3;
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

        if (windowPicker.SelectedItem.ToString() == "SLM")
        {
            var Widnow_310_Back_Page = App.ServiceProvider.GetRequiredService<MainPageAndroid>();
            await Navigation.PushAsync(Widnow_310_Back_Page);
        }
    }

    private void LoadIndicator(bool setActive)
    {
        LoginActivityIndicator.IsVisible = setActive;
        LoginActivityIndicator.IsRunning = setActive;
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

    private bool IsBlocked()
    {
        return LoginActivityIndicator.IsRunning;
    }

    private async Task PerformSearch(string query)
    {
        try
        {
            LoadIndicator(true);
            _searchResultsDictionary = await _dataManager.GetDataBySearchAsync(query);
            LoadIndicator(false);
            _searchResultValues.Clear();
            foreach (var resultValue in _searchResultsDictionary.Values)
            {
                _searchResultValues.Add(resultValue);
            }

            //SearchResultsListView.IsVisible = _searchResultValues.Count > 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось получить данные: {ex.Message}", "OK");
        }
    }

    private async void SearchResultsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (IsBlocked()) return;
        if (_storages.Count <= 0 && _storedSettings.Storages != null) _storages = _storedSettings.Storages;
        if (e.Item is string selectedValue)
        {
            var correspondingKey = _searchResultsDictionary.FirstOrDefault(i => i.Value == selectedValue).Key;
            LoadIndicator(true);
            var productData = await _dataManager.GetDataByIDAsync(correspondingKey, _storages.Values.ToArray());
            LoadIndicator(false);
            ArticleField.Text = productData.Article;
            ArtNameField.Text = productData.Name;
            QtyTitle.Text = "Всего на складе ТТ310 - ";
            ImageViewer.Source = productData.ImageUrl;

            //_addressViewModel.Addresses.Clear();
            _addressViewModel.Article = productData.Article;
            _selectedProduct = productData;

            if (productData != null && productData.Storages != null )
            {
                var index = Array.IndexOf(productData.Storages, "ТТ310");
                if (index > 0 && productData?.QtyInStorages?.Length > index) qtyInStore = (int)productData.QtyInStorages[index];
                else qtyInStore = 0;
                ArtQtyField.Text = qtyInStore.ToString();
            }

            _searchResultValues.Clear();
            SearchResultsListView.ItemsSource = _searchResultValues;
            if (productData != null) _productData = productData;
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (IsBlocked())
        {
            return;
        }
        await CreateMove();
    }

    private async Task CreateMove()
    {
        if (_selectedProduct == null)
        {
            await DisplayAlert("Ошибка", "Товар не выбран!", "OK");
            return;
        }

        List<ProductForMove> movingProducts = new List<ProductForMove>();

        //Проверяем, правильно ли пришла цена
        float convertedPrice = 0;
        if (!float.TryParse(_selectedProduct.Price, out convertedPrice))
        {
            await DisplayAlert("Ошибка", "Что-то пошло не так!", "OK");
            return;
        }

        //Проверяем правильно ли указано количество
        if (!float.TryParse(QtyField.Text, out float qty) || qtyInStore < qty)
        {
            await DisplayAlert("Ошибка", "Введите корректное количество!", "OK");
            return;
        }

        movingProducts.Add(new ProductForMove()
        {
            Id = _selectedProduct.ID,
            Article = _selectedProduct.Article,
            Name = _selectedProduct.Name,
            Price = convertedPrice,
            OrderedQty = QtyField.Text,
        });

        if (string.IsNullOrWhiteSpace(
            _storedSettings.MoveToStore310) || string.IsNullOrWhiteSpace(_storedSettings.MoveFromStore310))
            return;

        LoadIndicator(true);
        var res = await _dataManager.CreateMoveWithArticles(
            movingProducts,
            _storedSettings.MoveFromStore310,
            _storedSettings.MoveToStore310,
            _storedSettings.Username); // Здесь поменяно местами, надо будет переделать
        LoadIndicator(false);

        if (res)
        {
            RefreshAfterSuccess();
            await DisplayAlert("Успех", "Списание успешно!", "OK");
        }
        else
        {
            await DisplayAlert("Ошибка", "Списание не удалось", "OK");
        }
    }
    private void RefreshAfterSuccess()
    {
        ArticleField.Text = string.Empty;
        ArtNameField.Text = string.Empty;
        QtyTitle.Text = string.Empty;
        ImageViewer.Source = null;
        QtyField.Text = string.Empty;
        ArtQtyField.Text = string.Empty;
    }
}