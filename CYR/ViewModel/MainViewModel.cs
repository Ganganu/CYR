using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Dashboard.DashboardViewModels;
using CYR.Invoice.InvoiceViewModels;
using CYR.Messages;
using CYR.Services;
using CYR.User;
using System.Windows;

namespace CYR.ViewModel;

public partial class MainViewModel : ObservableRecipient, IRecipient<NavigateBackSource>, IRecipient<SnackbarMessage>
{
    private readonly UserRepository _userRepository;
    private readonly UserContext _userContext;
    public MainViewModel(INavigationService navigationService, UserRepository userRepository, UserContext userContext)
    {
        Navigation = navigationService;
        Messenger.RegisterAll(this);
        ShowSnackbar = Visibility.Collapsed;
        _userRepository = userRepository;
        _userContext = userContext;

        GetUser();
        Navigation.NavigateTo<DashboardViewModel>();
    }

    [ObservableProperty]
    private Visibility _showSnackbar;
    [ObservableProperty]
    private string? _snackbarMessage;
    [ObservableProperty]
    private string? _snackbarIcon;

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private User.User? _user;

    [RelayCommand]
    private void NavigateToClientView()
    {
        Navigation.NavigateTo<ClientViewModel>();
    }
    [RelayCommand]
    private void NavigateToArticleView()
    {
        Navigation.NavigateTo<ArticleViewModel>();
    }
    [RelayCommand]
    private void NavigateToSettingsView()
    {
        Navigation.NavigateTo<SettingsViewModel>();
    }
    [RelayCommand]
    private void NavigateToGetInvoices()
    {
        Navigation.NavigateTo<InvoiceListViewModel>();
    }
    [RelayCommand]
    private void NavigateToDashboard()
    {
        Navigation.NavigateTo<DashboardViewModel>();
    }

    /// <summary>
    /// Gesendet von einem UserControl, wenn aud Dashboard navigiert wird, um die Farbe des Buttons zu ändern.
    /// </summary>
    /// <param name="message"></param>
    public void Receive(NavigateBackSource message)
    {
        Navigation.NavigateTo<DashboardViewModel>();
    }

    public void Receive(SnackbarMessage message)
    {
        SnackbarMessage = message.Message;
        ShowSnackbar = Visibility.Visible;
        SnackbarIcon = message.Icon;
    }

    private async Task GetUser()
    {
        var user = await _userRepository.GetUserAsync("admin", "admin");
        User = user;
        _userContext.CurrentUser = user;
    }
}
