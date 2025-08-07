using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Services;
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
    public string? UserLogo => string.IsNullOrEmpty(UserCompany.UserLogo) ? @"/Ressources/user.png" : UserCompany.UserLogo;

    [RelayCommand]
    private void NavigateToUserView()
    {
        Navigation.NavigateTo<UserViewModel>();
    }
}
