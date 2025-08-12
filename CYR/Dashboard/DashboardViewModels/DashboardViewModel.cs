using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Login;
using CYR.Services;
using CYR.User;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly StatisticOverviewViewModel _statisticOverviewViewModel;
    private readonly StatisticChartViewModel _statisticChartViewModel;
    private readonly DashboardUserViewModel _dashboardUserViewModel;
    private readonly DashboardInvoiceViewModel _dashboardInvoiceViewModel;
    private readonly UserContext _userContext;
    private readonly UserRepository _userRepository;
    private readonly LoginRepository _loginRepository;
    private readonly ILoginTokenService _loginTokenService;



    public DashboardViewModel(StatisticOverviewViewModel statisticOverviewViewModel, StatisticChartViewModel statisticChartViewModel, DashboardUserViewModel dashboardUserViewModel, DashboardInvoiceViewModel dashboardInvoiceViewModel, INavigationService navigation, UserContext userContext, UserRepository userRepository, LoginRepository loginRepository, ILoginTokenService loginTokenService)
    {
        _statisticOverviewViewModel = statisticOverviewViewModel;
        _statisticChartViewModel = statisticChartViewModel;
        _dashboardUserViewModel = dashboardUserViewModel;
        _dashboardInvoiceViewModel = dashboardInvoiceViewModel;
        _navigation = navigation;
        _userContext = userContext;
        _userRepository = userRepository;
        Initialize();
        _loginRepository = loginRepository;
        _loginTokenService = loginTokenService;
    }


    private async Task Initialize()
    {
        if (_userContext.CurrentUser is null) return;
        string? username = _userContext.CurrentUser.Username;
        User = await _userRepository.GetUserAsync(username);
    }

    public StatisticOverviewViewModel StatisticOverviewVM => _statisticOverviewViewModel;
    public StatisticChartViewModel StatisticChartVM => _statisticChartViewModel;
    public DashboardUserViewModel DashboardUserVM => _dashboardUserViewModel;
    public DashboardInvoiceViewModel DashboardInvoiceVM => _dashboardInvoiceViewModel;

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private User.User? _user;
    public string? UserLogo => string.IsNullOrEmpty(User.Logo) ? @"/Ressources/user.png" : User.Logo;

    [RelayCommand]
    private void NavigateToUserView()
    {
        Navigation.NavigateTo<UserViewModel>();
    }
    [RelayCommand]
    private async Task Logout()
    {
        if (_userContext.CurrentUser is null)
            return;

        await _loginRepository.LogoutAsync(_userContext.CurrentUser.Username);

        _loginTokenService.DeleteToken();

        _userContext.CurrentUser = null;
        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
        if (!string.IsNullOrEmpty(exePath))
        {
            System.Diagnostics.Process.Start(exePath);
        }
        Application.Current.Shutdown();
    }
}
