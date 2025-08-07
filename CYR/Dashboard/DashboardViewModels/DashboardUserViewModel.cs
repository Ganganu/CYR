using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Services;
using CYR.Settings;
using CYR.User;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardUserViewModel : ObservableRecipient
{
    private readonly UserContext _userContext;
    private readonly UserCompanyRepository _userCompanyRepository;
    public DashboardUserViewModel(UserContext userContext, INavigationService navigation, UserCompanyRepository userCompanyRepository)
    {
        _userContext = userContext;
        _navigation = navigation;
        _userCompanyRepository = userCompanyRepository;

        Initialize();
    }

    private async Task Initialize()
    {
        if (_userContext.CurrentUser is null) return;
        int id = Convert.ToInt32(_userContext.CurrentUser.Id);
        UserCompany = await _userCompanyRepository.GetAsync(id);
    }

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private UserCompany? _userCompany;

    [ObservableProperty]
    private User.User? _user;
    [ObservableProperty]
    private Company _company;
    [ObservableProperty]
    private string? _userLogoSource;
    public string? UserLogo => string.IsNullOrEmpty(_userLogoSource) ? @"/Ressources/user.png" : UserLogoSource;

    [RelayCommand]
    private void NavigateToUserView()
    {
        Navigation.NavigateTo<UserViewModel>();
    }
}
