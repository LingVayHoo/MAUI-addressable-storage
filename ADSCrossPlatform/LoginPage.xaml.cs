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
        // Логика проверки логина
        bool loginSuccess = await IsLoginValid(UsernameEntry.Text, PasswordEntry.Text);
        //loginSuccess = true;
        if (loginSuccess)
        {
            // Получаем экземпляр MainPage из DI контейнера
            var mainPage = App.ServiceProvider.GetRequiredService<MainPageAndroid>();

            // Устанавливаем данные и открываем основную страницу
            mainPage.SetStorages(Storages);
            mainPage.AdminLogged();

            // Переход на главную страницу
            Application.Current.MainPage = new NavigationPage(mainPage);
        }
        else
        {
            await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
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
        // Логика проверки данных
        return true; // Логин успешен
    }
}
