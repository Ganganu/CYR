using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Services;

namespace CYR.Login;

public partial class LoginViewModel : ObservableRecipient
{
    private readonly LoginRepository _loginRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ILoginTokenService _loginTokenService;


    public LoginViewModel(LoginRepository loginRepository, IPasswordHasherService passwordHasherService, ILoginTokenService loginTokenService)
    {
        _loginRepository = loginRepository;
        IsViewVisible = true;
        _passwordHasherService = passwordHasherService;
        _loginTokenService = loginTokenService;
        TryAutoLogin();
    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private bool _isViewVisible;

    [ObservableProperty]
    private string _loginError;
    [ObservableProperty]
    private bool _isStayLoggedInChecked;
    [ObservableProperty]
    private bool _isRegistering;


    public async void TryAutoLogin()
    {
        var tokenData = _loginTokenService.LoadToken();
        if (tokenData != null)
        {
            bool success = await _loginRepository.LoginWithToken(tokenData.Value.Username, tokenData.Value.Token);
            if (success)
            {
                IsViewVisible = false;
            }
            else
            {
                _loginTokenService.DeleteToken(); // invalid token
            }
        }
    }


    [RelayCommand]
    private async Task Login()
    {
        LoginError = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            LoginError = "Bitte Benutzername und Passwort eingeben";
            return;
        }

        try
        {
            bool loginResult = await _loginRepository.Login(Username, Password);

            if (loginResult)
            {
                if (IsStayLoggedInChecked)
                {
                    // Generate a token (you can use Guid or server-generated token)
                    string token = Guid.NewGuid().ToString();
                    _loginTokenService.SaveToken(Username, token);

                    // Optionally tell server to associate token with user
                    await _loginRepository.RememberTokenAsync(Username, token);
                }

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

    [RelayCommand]
    private void CreateUser()
    {
        IsRegistering = true;
    }
}