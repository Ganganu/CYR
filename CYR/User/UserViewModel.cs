using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Dashboard.DashboardViewModels;
using CYR.Messages;
using CYR.Services;
using CYR.Settings;

namespace CYR.User;

public partial class UserViewModel : ObservableRecipient
{
    private readonly UserContext _userContext;
    private readonly ISelectImageService _selectImageService;
    private readonly UserRepository _userRepository;
    private readonly CompanyRepository _companyRepository;
    public UserViewModel(UserContext userContext, ISelectImageService selectImageService, INavigationService navigation, UserRepository userRepository, CompanyRepository companyRepository)
    {
        _userContext = userContext;
        _selectImageService = selectImageService;
        _navigation = navigation;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        Initialize();
    }

    private async Task Initialize()
    {
        if (_userContext.CurrentUser is null) return;
        int id = Convert.ToInt32(_userContext.CurrentUser.Id);
        string username = _userContext.CurrentUser.Username;
        User = await _userRepository.GetUserAsync(username);
        Company = await _companyRepository.GetCompanyAsync(id);
    }


    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private User? _user;
    [ObservableProperty]
    private Company? _company;

    [RelayCommand]
    private void SelectNewUserLogo()
    {
        var newLogo = _selectImageService.SelectStringImage();
        User.Logo = newLogo;
    }
    [RelayCommand]
    private void SelectNewCompanyLogo()
    {
        var newLogo = _selectImageService.SelectStringImage();
        Company.Logo = newLogo;
    }

    [RelayCommand]
    private async Task UpdateUser()
    {
        if (_userContext.CurrentUser is null) return;
        var resultuser = await _userRepository.InsertAsync(User);
        var resultCompany = await _companyRepository.InsertAsync(Company);
        if (resultuser > 0 && resultCompany > 0)
        {
            Messenger.Send(new SnackbarMessage(@$"Der Benutzer {User.Username} wurde erfolgreich aktualisiert!", "Check"));
        }
        else
        {
            Messenger.Send(new SnackbarMessage(@"Problem beim Aktulisieren der Benutzerdaten", "Error"));            
        }
    }
    [RelayCommand]
    private void NavigateBack()
    {
        Navigation.NavigateTo<DashboardViewModel>();
    }
}
