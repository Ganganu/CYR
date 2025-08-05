using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Services;
using CYR.ViewModel;

namespace CYR.Login;

public partial class RegisterViewModel : ObservableRecipient
{
    public RegisterViewModel(INavigationService navigation)
    {

    }

    [ObservableProperty]
    private string? _username;
    [ObservableProperty]
    private string _password;
    [ObservableProperty]
    private string _confirmPassword;

    [RelayCommand]
    private void CreateUser()
    {

    }
    [RelayCommand]
    private void NavigateBack()
    {
        Messenger.Send<NavigateToLoginMessage>();
    }
}

public record NavigateToLoginMessage();
