namespace ADSCrossPlatform;
using ADSCrossPlatform.Code.Models;

public partial class Details : ContentPage
{
    private AddressViewModel _addressViewModel;

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


    }

    private void InitFields()
    {
        if (_addressViewModel.SelectedAddressModel == null) return;

        _addressViewModel.SelectedAddressModel.Article = ArticleField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Zone = ZoneField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Row = RowField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Place = PlaceField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Level = LevelField.Text ?? string.Empty;
        _addressViewModel.SelectedAddressModel.Qty = QtyField.Text ?? string.Empty;        
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
        res = await _addressViewModel.EditRecord(_addressViewModel.SelectedAddressModel.AddressDBModel);
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
}