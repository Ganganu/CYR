using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CYR.Login;

public partial class LoginViewModel : ObservableRecipient
{
    private readonly LoginRepository _loginRepository;

    public LoginViewModel(LoginRepository loginRepository)
    {
        _loginRepository = loginRepository;
        IsViewVisible = true;
    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private bool _isViewVisible;

    [ObservableProperty]
    private string _loginError;

    [RelayCommand]
    private async Task Login()
    {
        LoginError = string.Empty;

        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            LoginError = "Bitte Benutzername und Passwort eingeben";
            return;
        }

        try
        {
            bool loginResult = await _loginRepository.Login(Username, Password);

            if (loginResult)
            {
                IsViewVisible = false;
            }
            else
            {
                LoginError = "Eingabe ungültig";
            }
        }
        catch (Exception ex)
        {
            LoginError = $"Fehler beim Anmelden: {ex.Message}";
        }
    }
}