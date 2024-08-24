namespace ADSCrossPlatform;
using ADSCrossPlatform.Code.Models;

public partial class Details : ContentPage
{
	public Details()
	{
		InitializeComponent();
	}

	public Details(AddressViewModel addressViewModel, AddressDBModel addressDBModel, bool isNew)
	{
		InitializeComponent();
        ArticleField.Text = addressDBModel.Article;
        ZoneField.Text = addressDBModel.Zone;
        RowField.Text = addressDBModel.Row;
        PlaceField.Text = addressDBModel.Place;
        LevelField.Text = addressDBModel.Level;
        QtyField.Text = addressDBModel.Qty;

        CancelButton.Clicked += async (o, e) =>
        {
            if (IsBlocked()) return;
            addressViewModel.SelectedAddressModel = null;
            await Navigation.PopAsync();
        };

        OkButton.Clicked += async delegate
        {
            if (IsBlocked()) return;
            addressDBModel.Article = ArticleField.Text;
            addressDBModel.Zone = ZoneField.Text;
            addressDBModel.Row = RowField.Text;
            addressDBModel.Place = PlaceField.Text;
            addressDBModel.Level = LevelField.Text;
            addressDBModel.Qty = QtyField.Text;
            bool res = false;
            IndicatorSetActive(true);
            if (isNew) res = await addressViewModel.CreateRecord(addressDBModel);
            else res = await addressViewModel.EditRecord(addressDBModel);
            addressViewModel.SelectedAddressModel = null;
            if (res) await Navigation.PopAsync();
            IndicatorSetActive(false);
        };

        DeleteButton.Clicked += async delegate
        {
            if (IsBlocked()) return;
            try
            {
                bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить элемент?", "Да", "Нет");
                if (result)
                {
                    addressDBModel.Article = ArticleField.Text;
                    addressDBModel.Zone = ZoneField.Text;
                    addressDBModel.Row = RowField.Text;
                    addressDBModel.Place = PlaceField.Text;
                    addressDBModel.Level = LevelField.Text;
                    addressDBModel.Qty = QtyField.Text;

                    IndicatorSetActive(true);
                    bool res = await addressViewModel.DeleteRecord(addressDBModel);
                    IndicatorSetActive(false);
                    addressViewModel.SelectedAddressModel = null;
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
        };



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
}