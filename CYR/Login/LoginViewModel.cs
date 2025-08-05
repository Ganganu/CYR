using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.User;

namespace CYR.Login;

public partial class LoginViewModel : ObservableRecipient, IRecipient<NavigateToLoginMessage>
{
    private readonly LoginRepository _loginRepository;
    private readonly ILoginTokenService _loginTokenService;
    private readonly UserContext _userContext;
    private readonly RegisterViewModel _registerViewModel;
    private readonly UserRepository _userRepository;


    public LoginViewModel(LoginRepository loginRepository, ILoginTokenService loginTokenService, RegisterViewModel registerViewModel, UserContext userContext, UserRepository userRepository)
    {
        _loginRepository = loginRepository;
        IsViewVisible = true;
        _loginTokenService = loginTokenService;
        TryAutoLogin();
        _registerViewModel = registerViewModel;
        Messenger.RegisterAll(this);
        _userContext = userContext;
        _userRepository = userRepository;
    }

    public RegisterViewModel RegisterViewModel => _registerViewModel;

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
                var user = await _userRepository.GetUserAsync(Username);
                _userContext.CurrentUser = user;
                if (IsStayLoggedInChecked)
                {
                    string token = Guid.NewGuid().ToString();
                    _loginTokenService.SaveToken(Username, token);

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

    public void Receive(NavigateToLoginMessage message)
    {
        IsRegistering = false;
    }
}