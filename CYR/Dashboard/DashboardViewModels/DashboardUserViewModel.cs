using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Services;
using CYR.User;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardUserViewModel : ObservableRecipient
{
    private readonly UserContext _userContext;
    private readonly UserRepository _userRepository;
    private readonly CompanyRepository _companyRepository;
    public DashboardUserViewModel(UserContext userContext, INavigationService navigation, UserRepository userRepository, CompanyRepository companyRepository)
    {
        _userContext = userContext;
        _navigation = navigation;
        _userRepository = userRepository;
        _companyRepository = companyRepository;

        Initialize();
    }

    private async Task Initialize()
    {
        if (_userContext.CurrentUser is null) return;
        string? username = _userContext.CurrentUser.Username;
        User = await _userRepository.GetUserAsync(username);
        int id = Convert.ToInt32(_userContext.CurrentUser.Id);
        try
        {
        Company = await _companyRepository.GetCompanyAsync(id);

        }
        catch (Exception)
        {

            throw;
        }
    }

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private User.User? _user;
    [ObservableProperty]
    private Company? _company;
    public string? UserLogo => string.IsNullOrEmpty(User.Logo) ? @"/Ressources/user.png" : User.Logo;

    [RelayCommand]
    private void NavigateToUserView()
    {
        Navigation.NavigateTo<UserViewModel>();
    }
}
