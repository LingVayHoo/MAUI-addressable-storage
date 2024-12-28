using ADSCrossPlatform.Code.Models;

namespace ADSCrossPlatform;

public partial class SACDetails : ContentPage
{
	private SACViewModel _model;
	public SACDetails(SACViewModel model)
	{
		InitializeComponent();
        _model = model;
        var r = new SAC()
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
        };
        BindingContext = r;

    }
}