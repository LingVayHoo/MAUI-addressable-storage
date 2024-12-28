using ADSCrossPlatform.Code.Models;
namespace ADSCrossPlatform;

public partial class NewAddressPage : ContentPage
{
    private AddressViewModel _addressViewModel;
    private AddressDBModel _addressDBModel;

    public Dictionary<string, string>? Storages { get; set; }

    public NewAddressPage(AddressViewModel addressViewModel)
    {
        InitializeComponent();

        _addressDBModel = new AddressDBModel();
        _addressViewModel = addressViewModel;



        if (_addressViewModel.SelectedAddressModel != null) 
            ArticleField.Text = _addressViewModel.SelectedAddressModel.Article;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        List<string> storagesList = new List<string>()
        {
            "���"
        };

        if (Storages != null) storagesList.AddRange(Storages.Values);

        StorePicker.ItemsSource = storagesList;
    }

    private void IndicatorSetActive(bool value)
    {
        LoadActivityIndicator.IsVisible = value;
        LoadActivityIndicator.IsRunning = value;
    }

    private bool IsBlocked()
    {
        return LoadActivityIndicator.IsRunning;
    }

    private async void OkButton_Clicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;
        _addressDBModel.Id = Guid.NewGuid();
        _addressDBModel.ProductID = _addressViewModel.SelectedAddressModel?.ProductID ?? string.Empty;
        _addressDBModel.ProductName = _addressViewModel.SelectedAddressModel?.ProductName ?? string.Empty;
        _addressDBModel.StoreID = StorePicker?.SelectedItem?.ToString() ?? string.Empty;
        _addressDBModel.Article = ArticleField.Text ?? string.Empty;
        _addressDBModel.Zone = ZoneField.Text ?? string.Empty;
        _addressDBModel.Row = RowField.Text ?? string.Empty;
        _addressDBModel.Place = PlaceField.Text ?? string.Empty;
        _addressDBModel.Level = LevelField.Text ?? string.Empty;
        _addressDBModel.Qty = QtyField.Text ?? string.Empty;
        _addressDBModel.IsPrimaryPlace = PrimaryCheckBox.IsChecked;
        _addressDBModel.IsSalesLocation = SalesCheckBox.IsChecked;
        bool res = false;
        IndicatorSetActive(true);
        try
        {
            res = await _addressViewModel.CreateRecord(_addressDBModel);
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", $"�� ������� ������� ������: {ex.Message}", "��");
            IndicatorSetActive(false);
            return;
        }

        _addressViewModel.SelectedAddressModel = null;
        IndicatorSetActive(false);
        if (res) await Navigation.PopAsync();
        else await DisplayAlert("������", $"�� ������� ������� ������", "��");
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;
        _addressViewModel.SelectedAddressModel = null;
        await Navigation.PopAsync();
    }
}