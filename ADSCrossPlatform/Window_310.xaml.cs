namespace ADSCrossPlatform;
using ADSCrossPlatform.Code.Models;
using System.Collections.ObjectModel;

public partial class Window_310 : ContentPage
{
    private DataManager _dataManager;
    private AddressViewModel _addressViewModel;
    private ProductData _productData;

    private Dictionary<string, string> _searchResultsDictionary;
    private ObservableCollection<string> _searchResultValues;
    private Dictionary<string, string> _storages;

    public Window_310()
	{
		InitializeComponent();

        // Инициализация полей
        _searchResultsDictionary = new Dictionary<string, string>();
        _searchResultValues = new ObservableCollection<string>();
        _storages = new Dictionary<string, string>();
        _productData = new ProductData();

        _dataManager = new(new WebApiModel());
        _addressViewModel = new(_dataManager);
        windowPicker.SelectedIndex = 0;
        SearchResultsListView.ItemsSource = _searchResultValues;
    }

    private async void PickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsBlocked())
        {
            windowPicker.SelectedIndex = 0;
            return;
        }
        if (windowPicker.SelectedItem.ToString() == "43")
        {
            await Navigation.PushAsync(new MainPageAndroid());
        }
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

    private bool IsBlocked()
    {
        return LoginActivityIndicator.IsRunning;
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
            if (productData != null) _productData = productData;
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;
        await DisplayAlert("Ошибка", "Списание не удалось", "OK");

        //CreateMove();
    }

    private async void CreateMove()
    {
        var t = new MyStorageMovement();
        int qty = 0;
        if (_productData.Article != ArticleField.Text)
        {
            await DisplayAlert("Ошибка", "Неверный артикул", "OK");
            return;
        }

        if (qty == 0)
        {
            await DisplayAlert("Ошибка", "Введите корректное количество!", "OK");
            return;
        }

        var r = await t.AddPositionToMevement(
            _productData.ID,
            qty);

        if (r.IsSuccessStatusCode)
        {
            await DisplayAlert("Успех", "Списание успешно!", "OK");
        }
        else
        {
            await DisplayAlert("Ошибка", "Списание не удалось", "OK");
        }
    }
}

    //private async void PickerSelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (windowPicker.SelectedItem.ToString() == "43") await Shell.Current.GoToAsync("MainPage");
    //    //await Navigation.PushAsync(new MainPage());
    //}

    //private void Button_Clicked(object sender, EventArgs e)
    //{


    //    if (Search(ArticleField.Text, true)) return;
    //    if (Search(ArticleField.Text, false)) return;

    //    //Попытка поиска с измененными данными по артикулу и имени
    //    ArticleField.Text = ArticleField.Text.Replace(".", null);
    //    if (Search(ArticleField.Text, true)) return;

    //    ArticleField.Text = ArticleField.Text.Replace(".", null);
    //    if (Search(ArticleField.Text, false)) return;
    //}

    //private bool Search(string art, bool isFindByArticle)
    //{
    //    _addressViewModel.UpdateArticle(art);
    //    var allInfo = _addressViewModel.GetAllInfo(art, isFindByArticle);
    //    ArtNameField.Text = allInfo[0];
    //    ArtQtyField.Text = allInfo[1];

    // var t = new MyStorageMovement();
    //    int qty = 0;
    //    double price = 0;
    //    int.TryParse(QtyField.Text, out qty);
    //    double.TryParse(allInfo[3], out price);
    //    if (qty == 0 || price == 0) return false;

    //    t.AddPositionToMevement(
    //        allInfo[2], 
    //        qty,
    //        price);

    //    return !(ArtNameField.Text == string.Empty);
    //}
