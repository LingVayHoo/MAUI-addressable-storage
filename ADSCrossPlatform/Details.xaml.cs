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

        CancelButton.Clicked += async(o, e) => await Navigation.PopAsync();

        OkButton.Clicked += async delegate
        {
            addressDBModel.Article = ArticleField.Text;
            addressDBModel.Zone = ZoneField.Text;
            addressDBModel.Row = RowField.Text;
            addressDBModel.Place = PlaceField.Text;
            addressDBModel.Level = LevelField.Text;
            addressDBModel.Qty = QtyField.Text;
            bool res = false;
            if (isNew) res = await addressViewModel.CreateRecord(addressDBModel);
            else res = await addressViewModel.EditRecord(addressDBModel);
            if (res) await Navigation.PopAsync();
        };

        DeleteButton.Clicked += async delegate
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
                bool res = false;
                res = await addressViewModel.DeleteRecord(addressDBModel);
                if (res) await Navigation.PopAsync();
            }
            
        };
    }
}