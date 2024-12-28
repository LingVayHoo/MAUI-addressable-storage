using ADS.Code.Export;
using ADSCrossPlatform.Code.Models;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace ADSCrossPlatform;

public partial class MainPageAndroid : ContentPage
{
    private readonly DataManager _dataManager;
    private readonly AddressViewModel _addressViewModel;
    private readonly StoredSettings _storedSettings;
    private readonly AddressHistoryViewModel _addressHistoryViewModel;
    private readonly ExportToExcelHandler _exportToExcelHandler;

    private Dictionary<string, string> _searchResultsDictionary;
    private ObservableCollection<string> _searchResultValues;
    private Dictionary<string, string> _storages;

    private ProductData? _productData;

    public MainPageAndroid(
        DataManager dataManager, 
        AddressViewModel addressViewModel, 
        StoredSettings storedSettings, 
        AddressHistoryViewModel addressHistoryViewModel,
        ExportToExcelHandler exportToExcelHandler)
    {
        InitializeComponent();

        windowPicker.SelectedIndex = 2;

        _dataManager = dataManager;
        _addressViewModel = addressViewModel;
        _storedSettings = storedSettings;
        _exportToExcelHandler = exportToExcelHandler;

        // ������������� �����
        _searchResultsDictionary = new Dictionary<string, string>();
        _searchResultValues = new ObservableCollection<string>();
        BindingContext = _addressViewModel;
        SearchResultsListView.ItemsSource = _searchResultValues;

        _storages = new Dictionary<string, string>();
        _addressHistoryViewModel = addressHistoryViewModel;

        // �������� ���������
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            // ������ ������ ��� Android
            PrenoteButton.IsVisible = false;
            SG010Button.IsVisible = false;
        }
    }

    public void SetStorages(Dictionary<string, string> storages)
    {
        _storages = storages;
        //2117cbc2-9f30-11ee-0a80-026e0010b8bf - TT390
        //8cbcce7c-aede-11ee-0a80-0dfd007bd0bc - TT310
        //8eecadfd-0367-11ed-0a80-0b430036e29c - �����������
        List<string> storagesList = new List<string>()
        {
            "���"
        };

        storagesList.AddRange(_storages.Values);

        storePicker.ItemsSource = storagesList;
    }

    public void InitSettings()
    {
        if (_storedSettings == null) return;
        _storedSettings.MoveToStore310 = _storedSettings.MoveToStore310 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "��310").Key : _storedSettings.MoveToStore310;
        _storedSettings.MoveFromStore310 = _storedSettings.MoveFromStore310 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "�����������").Key : _storedSettings.MoveFromStore310;
        _storedSettings.MoveToStore390 = _storedSettings.MoveToStore390 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "��390").Key : _storedSettings.MoveToStore390;
        _storedSettings.MoveFromStore390 = _storedSettings.MoveFromStore390 == string.Empty ?
            _storages.FirstOrDefault(x => x.Value == "�����������").Key : _storedSettings.MoveFromStore390;
    }

    public void AdminLogged()
    {
        // ��������� ��� �������
    }

    private bool IsBlocked()
    {
        return (LoginActivityIndicator.IsRunning || LoadActivityIndicator.IsRunning);
    }

    // ���������� ������� �� ������ "�����"
    private async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;
        string query = SearchField.Text;


        if (string.IsNullOrWhiteSpace(query))
        {
            await DisplayAlert("������", "������� ������� ��� ������", "OK");
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
            _productData = null;
            IndicatorSetActive(true);
            _searchResultsDictionary = 
                await _dataManager.GetDataBySearchAsync(query);
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
            await DisplayAlert("������", $"�� ������� �������� ������: {ex.Message}", "OK");
        }
    }

    private async void ToNewPage()
    {
        var newRecord = new AddressDBModel { Article = ArticleField.Text};

        _addressViewModel.SelectedAddressModel = new AddressModel(new AddressDBModel())
        {
            Article = ArticleField.Text,
            ProductID = IdField.Text,
            ProductName = ArtNameField.Text
        };

        var New_address_Page = App.ServiceProvider.GetRequiredService<NewAddressPage>();
        New_address_Page.Storages = _storages;
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
        if (!string.IsNullOrEmpty(ArticleField.Text))
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
            _productData = null;
            var correspondingKey = _searchResultsDictionary.FirstOrDefault(i => i.Value == selectedValue).Key;
            IndicatorSetActive(true);
            _productData = await _dataManager.GetDataByIDAsync(correspondingKey, _storages.Values.ToArray());
            IndicatorSetActive(false);
            ArticleField.Text = _productData.Article;
            ArtNameField.Text = _productData.Name;
            IdField.Text = _productData.ID;
            ImageViewer.Source = _productData.ImageUrl;
            
            //_addressViewModel.Addresses.Clear();
            _addressViewModel.Article = _productData.Article;

            ArtQtyField.Text = GetQtyInStorage(storePicker?.SelectedItem?.ToString() ?? "���", _productData);

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
            _addressViewModel.SelectedAddressModel.ProductID = IdField.Text;
            _addressViewModel.SelectedAddressModel.ProductName = ArtNameField.Text;
            var Details_Page = App.ServiceProvider.GetRequiredService<Details>();
            Details_Page.Storages = _storages;
            await Navigation.PushAsync(Details_Page);
        }
    }

    private async void History_Clicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;

        if (!string.IsNullOrEmpty(ArticleField.Text))
        {
            _addressHistoryViewModel.ArticleForHistory = ArticleField.Text;

            var history_Page = App.ServiceProvider.GetRequiredService<HistoryPage>();
            IndicatorSetActive(true);
            await Navigation.PushAsync(history_Page);
            IndicatorSetActive(false);
        }
    }

    private async void OnPrenoteClicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;

        IndicatorSetActive(true);

        var r = await _dataManager.GetPrenote("8eecadfd-0367-11ed-0a80-0b430036e29c");

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var filePath = Path.Combine(desktopPath, "Prenote.xlsx");

        _exportToExcelHandler.ExportPrenoteToExcel(r, filePath);

        IndicatorSetActive(false);

        await DisplayAlert("�����", $"Prenote ������� �����������!", "OK");
    }

    private async void OnSG010Clicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;

        IndicatorSetActive(true);

        var r = await _dataManager.GetSG010();

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var filePath = Path.Combine(desktopPath, "SG010.xlsx");

        _exportToExcelHandler.ExportSG010ToExcel(r, filePath);

        IndicatorSetActive(false);

        await DisplayAlert("�����", $"SG010 ������� �����������!", "OK");
    }

    private void SearchField_Completed(object sender, EventArgs e)
    {
        SearchButton.SendClicked();
    }

    private void StorePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsBlocked())
        {            
            return;
        }

        if (_productData == null)
        {
            ArtQtyField.Text = string.Empty;
            return;
        }

        ArtQtyField.Text = GetQtyInStorage(storePicker.SelectedItem.ToString() ?? "���", _productData);
        _addressViewModel.FilterText = storePicker.SelectedItem.ToString() ?? "���";
    }

    private string GetQtyInStorage(string storage, ProductData productData)
    {
        if (storage == "���")
        {
            return productData?.QtyInStorages?.Sum().ToString() ?? "0";
        }
        else
        {
            // ���������, ��� ������� �� null
            if (productData.Storages == null || productData.QtyInStorages == null)
                return "0";

            // ���� ������ ���������
            int index = Array.IndexOf(productData.Storages, storage);

            // ���� ��������� ������� � ������ � �������� ������� QtyInStorages
            if (index >= 0 && index < productData.QtyInStorages.Length)
                return $"{productData.QtyInStorages[index].ToString()}({productData?.QtyInStorages?.Sum().ToString() ?? "0"})";

            return "0"; // ���� ��������� �� �������
        }
    }

    private async void ArticleField_Tapped(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ArticleField.Text))
        {
            await Clipboard.SetTextAsync(ArticleField.Text); // �������� ����� � ����� ������
        }
    }

    private async void OnAllocationClicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;

        IndicatorSetActive(true);

        var r = await _dataManager.GetAllocation();

        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        var filePath = Path.Combine(desktopPath, "Allocation.xlsx");

        _exportToExcelHandler.ExportAllocationToExcel(r, filePath);

        IndicatorSetActive(false);

        await DisplayAlert("�����", $"Allocation ������� �����������!", "OK");
    }
}
