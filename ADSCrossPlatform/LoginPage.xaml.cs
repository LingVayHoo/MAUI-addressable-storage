using ADSCrossPlatform.Code.Login;
using ADSCrossPlatform.Code.Models;

namespace ADSCrossPlatform;

public partial class LoginPage : ContentPage
{
    private DataManager _dataManager;
    public Dictionary<string, string> Storages { get; set; }

    public LoginPage()
    {
        InitializeComponent();
        Storages = new Dictionary<string, string>();
        _dataManager = new DataManager(new WebApiModel());
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // ������ �������� ������
        bool loginSuccess = await IsLoginValid(UsernameEntry.Text, PasswordEntry.Text);
        //loginSuccess = true;
        if (loginSuccess)
        {
            // �������� ��������� MainPage �� DI ����������
            var mainPage = App.ServiceProvider.GetRequiredService<MainPageAndroid>();

            // ������������� ������ � ��������� �������� ��������
            mainPage.SetStorages(Storages);
            mainPage.AdminLogged();

            // ������� �� ������� ��������
            Application.Current.MainPage = new NavigationPage(mainPage);
        }
        else
        {
            await DisplayAlert("������", "������������ ����� ��� ������", "OK");
        }
    }

    private async Task<bool> IsLoginValid(string username, string password)
    {
        AccountData accountData = new AccountData();
        accountData.Username = username;
        accountData.Password = password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return false;
        LoginActivityIndicator.IsVisible = true;
        LoginActivityIndicator.IsRunning = true;
        Storages = await _dataManager.Login(accountData);
        LoginActivityIndicator.IsVisible = false;
        LoginActivityIndicator.IsRunning = false;
        return Storages != null ? true : false;
    }


    private bool CheckLogin()
    {
        // ������ �������� ������
        return true; // ����� �������
    }
}
