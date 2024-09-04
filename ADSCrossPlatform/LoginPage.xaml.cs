using ADSCrossPlatform.Code.Login;
using ADSCrossPlatform.Code.Models;

namespace ADSCrossPlatform;

public partial class LoginPage : ContentPage
{
    private readonly DataManager _dataManager;
    private readonly StoredSettings _storedSettings;
    private readonly SecureSettings _secureSettings;
    private LoginData? _loginData;
    private bool _isLoggedIn;
    private string? _tempPassword;
    private List<string> _blacklist;

    public LoginPage(DataManager dataManager, StoredSettings storedSettings, SecureSettings secureSettings)
    {
        InitializeComponent();
        _blacklist = new List<string>()
        {
            "Хуй", "Член", "Хер", "Гандон", "Еблан", "Дебил", "Пидор"
        };
        _dataManager = dataManager;
        _storedSettings = storedSettings;
        EnterShortCodeText.IsVisible = false;
        LogoutButton.IsVisible = false;
        DoneButton.IsVisible = false;
        _secureSettings = secureSettings;
        CheckData();
    }

    private void LoadIndicator(bool setActive)
    {
        LoginActivityIndicator.IsVisible = setActive;
        LoginActivityIndicator.IsRunning = setActive;
    }

    private async void CheckData()
    {
        string data = await _secureSettings.GetTokenAsync();
        if (string.IsNullOrEmpty(data))
            return;

        LoginData loginData = new()
        {
            UserName = data,
            OtherData = data,
        };
        LoadIndicator(true);
        UIForCheck(true);
        _isLoggedIn = await _dataManager.CheckData(loginData);
        UIForCheck(false);
        LoadIndicator(false);
        if (_isLoggedIn)
        {
            UsernameEntry.IsReadOnly = true;
            UsernameEntry.Text = _storedSettings.MemoID;
            EnterShortCodeText.IsVisible = true;
            EnterShortCodeText.Text = "Введи PIN";
            PasswordEntry.Placeholder = "PIN";
            LogoutButton.IsVisible = true;            
        }
    }

    private void UIForCheck(bool setActive)
    {
        EnterShortCodeText.IsVisible = setActive;
        EnterShortCodeText.Text = "Проверка авторизации";
        LogoutButton.IsVisible = !setActive;
        LoginButton.IsVisible = !setActive;        
        UsernameEntry.IsVisible = !setActive;
        PasswordEntry.IsVisible = !setActive;
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        var mainPage = App.ServiceProvider.GetRequiredService<MainPageAndroid>();

        if (_isLoggedIn)
        {
            if (PasswordEntry.Text == _storedSettings.Password)
            {
                ApplyStorages(mainPage);
                Application.Current.MainPage = new NavigationPage(mainPage);
            }
            else
            {
                await DisplayAlert("Ошибка", "Неправильный пароль", "OK");
            }
        }
        else
        {
            if (await IsLoginValid(UsernameEntry.Text, PasswordEntry.Text))
            {
                LoadIndicator(true);
                await SettingsUpdate();
                LoadIndicator(false);

                ApplyStorages(mainPage);
                mainPage.AdminLogged();
                SetPassword();
                //Application.Current.MainPage = new NavigationPage(mainPage);
            }
            else
            {
                await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
            }
        }
    }

    private async Task<bool> IsLoginValid(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return false;

        LoadIndicator(true);
        //if (username == "bugrdan@akvatoria") 
        _loginData = await _dataManager.Login(new AccountData { Username = username, Password = password });

        LoadIndicator(false);

        return _loginData != null;
    }

    private void GuysFromStorage()
    {

    }

    private async Task SettingsUpdate()
    {
        if (_loginData == null) return;

        await _secureSettings.SaveTokenAsync(_loginData.OtherData);
        _storedSettings.Username = _loginData.UserName;

        if (_loginData.Storages != null)
        {
            _storedSettings.Storages = _loginData.Storages;
            _storedSettings.MoveToStore310 = _loginData.Storages.FirstOrDefault(i => i.Value == "ТТ310").Key;
            _storedSettings.MoveFromStore310 = _loginData.Storages.FirstOrDefault(i => i.Value == "ЧернаяРечка").Key;
            _storedSettings.MoveToStore390 = _loginData.Storages.FirstOrDefault(i => i.Value == "ТТ390").Key;
            _storedSettings.MoveFromStore390 = _loginData.Storages.FirstOrDefault(i => i.Value == "ЧернаяРечка").Key;
            _storedSettings.Password = _tempPassword ?? string.Empty;
        }

        LoadIndicator(true);
        _storedSettings.SaveData();
        LoadIndicator(false);
    }

    private void ApplyStorages(MainPageAndroid mainPage)
    {
        if (_storedSettings.Storages != null)
        {
            mainPage.SetStorages(_storedSettings.Storages);
        }
    }

    private void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        UsernameEntry.IsReadOnly = false;
        UsernameEntry.Text = string.Empty;
        UsernameEntry.Placeholder = "Username";
        PasswordEntry.Placeholder = "Password";
        EnterShortCodeText.IsVisible = false;
        LogoutButton.IsVisible = false;
        _isLoggedIn = false;
    }

    private async void OnDoneButtonClicked(object sender, EventArgs e)
    {
        var mainPage = App.ServiceProvider.GetRequiredService<MainPageAndroid>();

        if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
        {
            await DisplayAlert("Ошибка", "Имя все еще не введено!", "OK");
            return;
        }

        if (ContainsProfanity(UsernameEntry.Text))
        {
            await DisplayAlert("Ошибка", "Не страдай ерундой!", "OK");
            return;
        }

        if (!string.IsNullOrEmpty(_tempPassword))
        {
            // Проверка введенного кода
            if (PasswordEntry.Text == _tempPassword)
            {
                _storedSettings.Password = _tempPassword;
                _storedSettings.MemoID = UsernameEntry.Text;
                LoadIndicator(true);
                _storedSettings.SaveData();
                LoadIndicator(false);
                Application.Current.MainPage = new NavigationPage(mainPage);
            }
            else
            {
                await DisplayAlert("Ошибка", "Неправильный PIN-код", "OK");
            }
        }
        else
        {
            // Проверка, что код состоит из 4 цифр
            if (IsValidPin(PasswordEntry.Text))
            {
                _tempPassword = PasswordEntry.Text;
                EnterShortCodeText.Text = "Еще раз";
                PasswordEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Ошибка", "PIN-код должен состоять из 4 цифр", "OK");
                PasswordEntry.Text = string.Empty;
            }
        }
    }

    private bool ContainsProfanity(string input)
    {
        foreach (var badWord in _blacklist)
        {
            if (input.IndexOf(badWord, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true; // Обнаружено запрещенное слово
            }
        }
        return false; // Запрещенных слов не обнаружено
    }

    private bool IsValidPin(string pin)
    {
        // Проверяем, что введенный PIN-код состоит из 4 цифр
        return pin.Length == 4 && pin.All(char.IsDigit);
    }

    private void SetPassword()
    {
        EnterShortCodeText.IsVisible = true;
        DoneButton.IsVisible = true;
        LoginButton.IsVisible = false;
        LogoutButton.IsVisible = false;
        UsernameEntry.Text = string.Empty;
        //PasswordEntry.IsVisible = false;
        UsernameEntry.Placeholder = "Имя/Memo";
        PasswordEntry.Text = string.Empty;
        PasswordEntry.Placeholder = "PIN";
        EnterShortCodeText.Text = "Введи имя и придумай PIN";
    }

}
