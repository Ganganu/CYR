using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Logging;
using CYR.Login;
using CYR.Services;
using CYR.User;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardViewModel : ObservableRecipient
{
    private readonly StatisticOverviewViewModel _statisticOverviewViewModel;
    private readonly StatisticChartViewModel _statisticChartViewModel;
    private readonly DashboardActivityViewModel _dashboardActivityVM;
    private readonly DashboardInvoiceViewModel _dashboardInvoiceViewModel;
    private readonly UserContext _userContext;
    private readonly UserRepository _userRepository;
    private readonly LoginRepository _loginRepository;
    private readonly ILoginTokenService _loginTokenService;
    private readonly LoggingRepository _loggingRepository;
    private readonly CompanyRepository _companyRepository;


    public DashboardViewModel(StatisticOverviewViewModel statisticOverviewViewModel, StatisticChartViewModel statisticChartViewModel, DashboardActivityViewModel dashboardActivityVM, DashboardInvoiceViewModel dashboardInvoiceViewModel, INavigationService navigation, UserContext userContext, UserRepository userRepository, LoginRepository loginRepository, ILoginTokenService loginTokenService, LoggingRepository loggingRepository, CompanyRepository companyRepository)
    {
        _statisticOverviewViewModel = statisticOverviewViewModel;
        _statisticChartViewModel = statisticChartViewModel;
        _dashboardActivityVM = dashboardActivityVM;
        _dashboardInvoiceViewModel = dashboardInvoiceViewModel;
        _navigation = navigation;
        _userContext = userContext;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        Initialize();
        _loginRepository = loginRepository;
        _loginTokenService = loginTokenService;
        _loggingRepository = loggingRepository;
    }


    private async Task Initialize()
    {
        if (_userContext.CurrentUser is null) return;
        string? username = _userContext.CurrentUser.Username;
        int? id = Convert.ToInt32(_userContext.CurrentUser.Id);
        User = await _userRepository.GetUserAsync(username);
        Company = await _companyRepository.GetCompanyAsync(id.Value);
        if (Company.Name is null) IsWarningVisible = true;
    }

    public StatisticOverviewViewModel StatisticOverviewVM => _statisticOverviewViewModel;
    public StatisticChartViewModel StatisticChartVM => _statisticChartViewModel;
    public DashboardActivityViewModel DashboardActivityVM => _dashboardActivityVM;
    public DashboardInvoiceViewModel DashboardInvoiceVM => _dashboardInvoiceViewModel;

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private Company? _company;
    [ObservableProperty]
    private User.User? _user;
    [ObservableProperty]
    private bool? _isWarningVisible;
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
        await _loggingRepository.InsertAsync(CreateHisModel(_userContext));
        _loginTokenService.DeleteToken();

        _userContext.CurrentUser = null;
        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
        if (!string.IsNullOrEmpty(exePath))
        {
            System.Diagnostics.Process.Start(exePath);
        }
        Application.Current.Shutdown();
    }

    private HisModel CreateHisModel(UserContext userContext)
    {
        HisModel model = new HisModel();
        model.LoggingType = LoggingType.UserLogout;
        model.UserId = _userContext.CurrentUser.Id;
        model.Message = $@"User: {model.UserId} abgemeldet.";
        return model;
    }
}
