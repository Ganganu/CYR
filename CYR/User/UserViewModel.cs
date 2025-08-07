using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Dashboard.DashboardViewModels;
using CYR.Services;
using CYR.Settings;

namespace CYR.User;

public partial class UserViewModel : ObservableRecipient
{
    private readonly UserContext _userContext;
    private readonly UserCompanyRepository _userCompanyRepository;
    private readonly ISelectImageService _selectImageService;
    public UserViewModel(UserContext userContext, UserCompanyRepository userCompanyRepository, ISelectImageService selectImageService, INavigationService navigation)
    {
        _userContext = userContext;
        _userCompanyRepository = userCompanyRepository;
        Initialize();
        _selectImageService = selectImageService;
        _navigation = navigation;
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

    [RelayCommand]
    private void SelectNewUserLogo()
    {
        var newLogo = _selectImageService.SelectStringImage();
        UserCompany = UserCompany with { UserLogo = newLogo};
    }
    [RelayCommand]
    private void SelectNewCompanyLogo()
    {
        var newLogo = _selectImageService.SelectStringImage();
        UserCompany = UserCompany with { CompanyLogo = newLogo };
    }

    [RelayCommand]
    private void UpdateUser()
    {
        if (_userContext.CurrentUser is null) return;
        if (UserCompany is null) return; 
        _ = _userCompanyRepository.UpdateUserAndCompanyInTransactionAsync(UserCompany);
    }
    [RelayCommand]
    private void NavigateBack()
    {
        Navigation.NavigateTo<DashboardViewModel>();
    }
}
