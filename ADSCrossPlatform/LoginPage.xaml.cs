using ADS.Code.Login;
using ADSCrossPlatform.Code.Login;
using ADSCrossPlatform.Code.Models;
using Newtonsoft.Json;
using System;

namespace ADSCrossPlatform;

public partial class LoginPage : ContentPage
{
    private readonly DataManager _dataManager;
    private readonly StoredSettings _storedSettings;
    private readonly SecureSettings _secureSettings;
    private LoginData? _loginData;
    private bool _isLoggedIn;
    private string? _tempPassword;
    private string? _tempMainPassword;
    private List<string> _blacklist;

    public LoginPage(DataManager dataManager, StoredSettings storedSettings, SecureSettings secureSettings)
    {
        InitializeComponent();
        _blacklist = new List<string>()
        {
            "���", "����", "���", "������", "�����", "�����", "�����"
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
            UserName = _storedSettings.MemoID,
            OtherData = data,
            Storages = _storedSettings.Storages,
        };
        LoadIndicator(true);
        UIForCheck(true);
        var r = await _dataManager.CheckData(loginData);

        if (r.IsSuccessStatusCode)
        {
            _isLoggedIn = true;
            UIForCheck(false);
            LoadIndicator(false);
            if (_isLoggedIn)
            {
                UsernameEntry.IsReadOnly = true;
                UsernameEntry.Text = _storedSettings.MemoID;
                EnterShortCodeText.IsVisible = true;
                EnterShortCodeText.Text = "����� PIN";
                PasswordEntry.Placeholder = "PIN";
                LogoutButton.IsVisible = true;
            }
        }
        else if (r.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            UIForCheck(false);
            LoadIndicator(false);
        }
        else
        {
            UIForCheck(false);
            LoadIndicator(false);
            await DisplayAlert("������", $"������ ����������", "OK");
            CheckData();
        }
        
    }

    private void UIForCheck(bool setActive)
    {
        EnterShortCodeText.IsVisible = setActive;
        EnterShortCodeText.Text = "�������� �����������";
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
                await DisplayAlert("������", "������������ ������", "OK");
            }
        }
        else
        {
            if (await IsLoginValid(UsernameEntry.Text, PasswordEntry.Text))
            {
                LoadIndicator(true);
                await SettingsUpdate();
                LoadIndicator(false);

                mainPage.AdminLogged();
                ApplyStorages(mainPage);
                SetPassword();
                //Application.Current.MainPage = new NavigationPage(mainPage);
            }
            else
            {
                await DisplayAlert("������", "������������ ����� ��� ������", "OK");
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
        _tempMainPassword = password;
        LoadIndicator(false);

        return _loginData != null;
    }

    private async Task SettingsUpdate()
    {
        if (_loginData == null) return;

        if (!string.IsNullOrEmpty(_loginData.OtherData)) await _secureSettings.SaveTokenAsync(_loginData.OtherData);

        _storedSettings.Username = _loginData.UserName;

        if (_loginData.Storages != null)
        {
            _storedSettings.Storages = _loginData.Storages;
            _storedSettings.MoveToStore310 = _loginData.Storages.FirstOrDefault(i => i.Value == "��310").Key;
            _storedSettings.MoveFromStore310 = _loginData.Storages.FirstOrDefault(i => i.Value == "�����������").Key;
            _storedSettings.MoveToStore390 = _loginData.Storages.FirstOrDefault(i => i.Value == "��390").Key;
            _storedSettings.MoveFromStore390 = _loginData.Storages.FirstOrDefault(i => i.Value == "�����������").Key;
            _storedSettings.Password = _tempPassword ?? string.Empty;
        }

        LoadIndicator(true);
        _storedSettings.SaveData();
        LoadIndicator(false);

        return;
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
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
        {
            await DisplayAlert("������", "��� ��� ��� �� �������!", "OK");
            return;
        }

        if (ContainsProfanity(UsernameEntry.Text))
        {
            await DisplayAlert("������", "�� ������� �������!", "OK");
            return;
        }

        if (!string.IsNullOrEmpty(_tempPassword))
        {
            // �������� ���������� ����
            if (PasswordEntry.Text == _tempPassword)
            {
                _storedSettings.Password = _tempPassword;
                _storedSettings.MemoID = UsernameEntry.Text;
                LoadIndicator(true);
                var r = await _dataManager.LoginWithMemo(new AccountDataWithMemo()
                {
                    MemoID = UsernameEntry.Text,
                    Username = _loginData?.UserName ?? string.Empty,
                    Password = _tempMainPassword ?? string.Empty

                });

                if (r == null)
                {
                    await DisplayAlert("������", "���-�� ����� �� ���", "OK");
                    return;
                }

                await _secureSettings.SaveTokenAsync(r.OtherData);

                _storedSettings.SaveData();
                LoadIndicator(false);
                var mainPage = App.ServiceProvider.GetRequiredService<MainPageAndroid>();
                ApplyStorages(mainPage);
                Application.Current.MainPage = new NavigationPage(mainPage);
            }
            else
            {
                await DisplayAlert("������", "������������ PIN-���", "OK");
            }
        }
        else
        {
            // ��������, ��� ��� ������� �� 4 ����
            if (IsValidPin(PasswordEntry.Text))
            {
                _tempPassword = PasswordEntry.Text;
                EnterShortCodeText.Text = "��� ���";
                PasswordEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("������", "PIN-��� ������ �������� �� 4 ����", "OK");
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
                return true; // ���������� ����������� �����
            }
        }
        return false; // ����������� ���� �� ����������
    }

    private bool IsValidPin(string pin)
    {
        // ���������, ��� ��������� PIN-��� ������� �� 4 ����
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
        UsernameEntry.Placeholder = "���/Memo";
        PasswordEntry.Text = string.Empty;
        PasswordEntry.Placeholder = "PIN";
        EnterShortCodeText.Text = "����� ��� � �������� PIN";
        PasswordEntry.Completed -= PasswordEntry_Completed;
        PasswordEntry.Completed += PasswordEntry_FirstEntered;
    }

    private void PasswordEntry_Completed(object sender, EventArgs e)
    {
        LoginButton.SendClicked();
    }

    private void PasswordEntry_FirstEntered(object sender, EventArgs e)
    {
        DoneButton.SendClicked();
    }
}

//_loginData = await _dataManager.LoginWithMemo(new AccountDataWithMemo()
//{
//    MemoID = UsernameEntry.Text,
//            Username = _loginData.UserName, 
//            Password = _tempMainPassword ?? string.Empty
//        });

//        if (_loginData == null) return false;
