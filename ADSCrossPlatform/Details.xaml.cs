namespace ADSCrossPlatform;
using ADSCrossPlatform.Code.Models;

public partial class Details : ContentPage
{
    private AddressViewModel _addressViewModel;

    public Dictionary<string, string>? Storages { get; set; }

    public Details(AddressViewModel addressViewModel)
	{
		InitializeComponent();

        _addressViewModel = addressViewModel;

        ArticleField.Text = _addressViewModel.SelectedAddressModel?.Article;
        ZoneField.Text = _addressViewModel.SelectedAddressModel?.Zone;
        RowField.Text = _addressViewModel.SelectedAddressModel?.Row;
        PlaceField.Text = _addressViewModel.SelectedAddressModel?.Place;
        LevelField.Text = _addressViewModel.SelectedAddressModel?.Level;
        QtyField.Text = _addressViewModel.SelectedAddressModel?.Qty;
        PrimaryCheckBox.IsChecked = _addressViewModel.SelectedAddressModel?.IsPrimaryPlace ?? false;
        SalesCheckBox.IsChecked = _addressViewModel.SelectedAddressModel?.IsSalesLocation ?? false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        List<string> storagesList = new List<string>()
        {
            "Все"
        };

        if (Storages != null) storagesList.AddRange(Storages.Values);

        StorePicker.ItemsSource = storagesList;

        SetStorePickerValue();
    }

    private void InitFields()
    {
        if (_addressViewModel.SelectedAddressModel == null) return;

        _addressViewModel.SelectedAddressModel.Article = ArticleField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.StoreID = StorePicker?.SelectedItem?.ToString() ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Zone = ZoneField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Row = RowField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Place = PlaceField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Level = LevelField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Qty = QtyField.Text ?? string.Empty;     
        _addressViewModel.SelectedAddressModel.IsPrimaryPlace = PrimaryCheckBox.IsChecked;
        _addressViewModel.SelectedAddressModel.IsSalesLocation = SalesCheckBox.IsChecked;
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
        if (_addressViewModel.SelectedAddressModel == null)
        {
            await DisplayAlert("Ошибка!", "Что-то пошло не так :( Попробуйте снова", "Ok");
            return;
        }
        InitFields();
        bool res = false;
        IndicatorSetActive(true);
        try
        {
            res = await _addressViewModel.EditRecord(_addressViewModel.SelectedAddressModel.AddressDBModel);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось изменить запись: {ex.Message}", "ОК");
            IndicatorSetActive(false);
            return;
        }

        _addressViewModel.SelectedAddressModel = null;
        if (res) await Navigation.PopAsync();
        IndicatorSetActive(false);
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;
        _addressViewModel.SelectedAddressModel = null;
        await Navigation.PopAsync();
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (IsBlocked()) return;
        try
        {
            bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
            if (result)
            {
                if (_addressViewModel.SelectedAddressModel == null)
                {
                    await DisplayAlert("Ошибка!", "Что-то пошло не так :( Попробуйте снова", "Ok");
                    return;
                }

                InitFields();
                bool res = false;
                IndicatorSetActive(true);

                try
                {
                    res = await _addressViewModel.DeleteRecord(_addressViewModel.SelectedAddressModel.AddressDBModel); ;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Не удалось удалить запись: {ex.Message}", "ОК");
                    IndicatorSetActive(false);
                    return;
                }

                IndicatorSetActive(false);

                _addressViewModel.SelectedAddressModel = null;
                if (res)
                {
                    await Navigation.PopAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
        }
    }

    private void SetStorePickerValue()
    {
        foreach (var item in StorePicker.Items)
        {
            if (_addressViewModel == null || _addressViewModel.SelectedAddressModel == null) continue;

            if (item == _addressViewModel.SelectedAddressModel.StoreID)
            {
                StorePicker.SelectedItem = item;
                break;
            }
        }
    }
}