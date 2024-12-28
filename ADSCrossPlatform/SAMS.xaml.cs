using ADSCrossPlatform.Code.Models;
using System.Collections.ObjectModel;

namespace ADSCrossPlatform;

public partial class SAMS : ContentPage
{
    private SACViewModel _model;
	public SAMS(SACViewModel model)
	{
		InitializeComponent();
        _model = model;
		Test();

    }

	private void Test()
	{
		List<SAC> sACs = new List<SAC>()
		{
			new SAC()
			{
				Name = "Test Name",
				CreatedTime = DateTime.Now,
				UpdatedTime = DateTime.Now,
				Status = "Created",
				Photos = new List<string>
				{
                    "https://online.moysklad.ru/app/download/022d1d80-2376-4ad0-b836-b4a2a5ea2726",
                    "https://online.moysklad.ru/app/download/022d1d80-2376-4ad0-b836-b4a2a5ea2726"
                }
			},

            new SAC()
            {
                Name = "Test Name Two",
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Status = "Updated",
                Photos = new List<string>
                {
                    "https://online.moysklad.ru/app/download/022d1d80-2376-4ad0-b836-b4a2a5ea2726",
                    "https://online.moysklad.ru/app/download/022d1d80-2376-4ad0-b836-b4a2a5ea2726"
                }
            },
        };
        _model.Sacs = sACs;

        SearchResultsList.ItemsSource = new ObservableCollection<SAC>(_model.Sacs);

    }

    private async void SearchResultsListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is SAC selectedSac)
        {
            _model.SelectedSAC = selectedSac;
            // Открываем новую страницу и передаем выбранную запись SAC
            var mainPage = App.ServiceProvider.GetRequiredService<SACDetails>();
            await Navigation.PushAsync(mainPage);
        }
    }
}