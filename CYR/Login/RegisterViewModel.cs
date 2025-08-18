using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Dialog;
using CYR.Logging;
using CYR.User;

namespace CYR.Login;

public partial class RegisterViewModel : ObservableRecipient
{
    private readonly UserRepository _userRepository;
    private readonly IDialogService _dialogService;
    private string? _dialogResponse;
    public RegisterViewModel(UserRepository userRepository, IDialogService dialogService)
    {
        Roles = ["Admin", "User"];
        ErrorTexts = [];
        _userRepository = userRepository;
        _dialogService = dialogService;
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
        var succes = false;
        ErrorTexts?.Clear();
        if (Password != ConfirmPassword) ErrorTexts?.Add(@"Die Passwörter stimmen nicht überein. Bitte wiederholen Sie Ihre Eingabe.");
        if (SelectedRole is null || string.IsNullOrEmpty(SelectedRole)) ErrorTexts?.Add("Wählen sie eine Role.");
        ErrorText = ErrorTexts?.Count > 0 ? string.Join(Environment.NewLine, ErrorTexts) : string.Empty;

        User.User registerModel = new();
        if (ErrorTexts?.Count == 0)
        {
            int result;
            registerModel = new User.User();
            registerModel.Username = Username;
            registerModel.Password = Password;
            registerModel.Role = SelectedRole;
            try
            {
                result = await _userRepository.InsertAsync(registerModel);
            }
            catch (SQLiteException ex)
            {
                SQLiteErrorCodes errorCode = (SQLiteErrorCodes)Convert.ToInt32(ex.ErrorCode);
                ShowNotificationDialog("Konto erstellen", @$"Problem aufgetreten beim Erstellen des neuen Kontos! {Environment.NewLine}Fehler:{errorCode}", "Ok", "Error", Visibility.Collapsed, "");
                return;
            }
            succes = result > 0;
        }
        if (succes)
        {
            ShowNotificationDialog("Konto erstellen", "Konto erfolgreich erstellt.", "Ok", "Check", Visibility.Collapsed, "");
            Username = "";
            Password = "";
            ConfirmPassword = "";
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

    private void ShowNotificationDialog(string title,
        string message,
        string cancelButtonText,
        string icon,
        Visibility okButtonVisibility, string okButtonText)
    {
        _dialogService.ShowDialog(result =>
        {
            _dialogResponse = result;
        },
        new Dictionary<Expression<Func<NotificationViewModel, object>>, object>
        {
            { vm => vm.Title, title },
            { vm => vm.Message,  message},
            { vm => vm.CancelButtonText, cancelButtonText },
            { vm => vm.Icon,icon },
            { vm => vm.OkButtonText, okButtonText },
            { vm => vm.IsOkVisible, okButtonVisibility}
        });
    }
}

public record NavigateToLoginMessage();