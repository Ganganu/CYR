using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.User;

namespace CYR.Login;

public partial class RegisterViewModel : ObservableRecipient
{
    private readonly UserRepository _userRepository;
    public RegisterViewModel(UserRepository userRepository)
    {
        Roles = ["Admin", "User"];
        ErrorTexts = new List<string>();
        _userRepository = userRepository;
    }

    [ObservableProperty]
    private string? _username;

    [ObservableProperty]
    private string? _password;

    [ObservableProperty]
    private string? _confirmPassword;
    [ObservableProperty]
    private List<string>? _errorTexts;
    [ObservableProperty]
    private string? _errorText;
    [ObservableProperty]
    private ObservableCollection<string> _roles;
    [ObservableProperty]
    private string? _selectedRole;

    [RelayCommand(CanExecute = nameof(CanCreateUser))]
    private async Task CreateUser()
    {
        ErrorTexts?.Clear();
        if (Password != ConfirmPassword) ErrorTexts?.Add(@"Die Eingabe stimmt nicht überein mit dem eingegebenen Kennwort.");
        if (SelectedRole is null || string.IsNullOrEmpty(SelectedRole)) ErrorTexts?.Add("Wählen sie eine Role."); 
        ErrorText = ErrorTexts?.Count > 0 ? string.Join(Environment.NewLine, ErrorTexts) : string.Empty;

        if (ErrorTexts?.Count == 0)
        {
            User.User registerModel = new User.User();
            registerModel.Username = Username;
            registerModel.Password = Password;
            registerModel.Role= SelectedRole;
            var response = await _userRepository.InsertAsync(registerModel);
        }
    }

    private bool CanCreateUser()
    {
        return !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(ConfirmPassword);
    }

    partial void OnUsernameChanged(string? value)
    {
        CreateUserCommand.NotifyCanExecuteChanged();
    }

    partial void OnPasswordChanged(string? value)
    {
        CreateUserCommand.NotifyCanExecuteChanged();
    }

    partial void OnConfirmPasswordChanged(string? value)
    {
        CreateUserCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void NavigateBack()
    {
        Messenger.Send<NavigateToLoginMessage>();
    }
}

public record NavigateToLoginMessage();