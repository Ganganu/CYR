using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Dashboard.DashboardViewModels;
using CYR.Messages;
using CYR.Services;
using CYR.Settings;

namespace CYR.User;

public partial class UserViewModel : ObservableRecipientWithValidation
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
        InitializeUser();
        InitializeComapny();
    }
    private void InitializeUser()
    {
        if (User is null) return;
        Id = Convert.ToInt32(User.Id);
        Username = User.Username;
        UserLogo = User.Logo;
        Role = User.Role;
        Password = User.Password;
    }
    private void InitializeComapny()
    {
        if (Company is null) return;
        CompanyLogo = Company.Logo;
        CompanyName = Company.Name;
        CompanyStreet = Company.Street;
        CompanyCity = Company.City;
        CompanyPlz = Company.Plz;
        CompanyHouseNumber = Company.HouseNumber;
        CompanyTelefonNumber = Company.TelefonNumber;
        CompanyEmailAddress = Company.EmailAddress;
        CompanyBankName = Company.BankName;
        CompanyIban = Company.Iban;
        CompanyBic = Company.Bic;
        CompanyUstidnr = Company.Ustidnr;
        CompanyStnr = Company.Stnr;
        CompanyLogo = Company.Logo;
    }

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private User? _user;
    [ObservableProperty]
    private Company? _company;

    [ObservableProperty]
    private int? _id;
    [ObservableProperty]
    private string _username;
    [ObservableProperty]
    private string _userLogo;
    [ObservableProperty]
    private string _role;
    [ObservableProperty]
    private string _password;


    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyName;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyStreet;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyCity;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyPlz;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyHouseNumber;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyTelefonNumber;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyEmailAddress;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyBankName;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyIban;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyBic;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyUstidnr;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyStnr;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _companyLogo;


    [RelayCommand]
    private void SelectNewUserLogo()
    {
        var newLogo = _selectImageService.SelectStringImage();
        UserLogo = newLogo;
    }
    [RelayCommand]
    private void SelectNewCompanyLogo()
    {
        var newLogo = _selectImageService.SelectStringImage();
        CompanyLogo = newLogo;
    }

    [RelayCommand]
    private async Task UpdateUser()
    {
        if (_userContext.CurrentUser is null) return;
        var resultuser = await _userRepository.InsertAsync(CreateUser());
        var resultCompany = await _companyRepository.InsertAsync(CreateCompany());
        if (resultuser > 0 && resultCompany > 0)
        {
            Messenger.Send(new SnackbarMessage(@$"Der Benutzer {Username} wurde erfolgreich aktualisiert!", "Check"));
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
    private User CreateUser()
    {
        User user = new();
        user.Id = Id.ToString();
        user.Username = Username;
        user.Logo = UserLogo;
        user.Role = Role;
        user.Password = Password;
        return user;
    }
    private Company CreateCompany()
    {
        Company company = new Company();
        company.Name = CompanyName;
        company.Street = CompanyStreet;
        company.City = CompanyCity;
        company.Plz = CompanyPlz;
        company.HouseNumber = CompanyHouseNumber;
        company.TelefonNumber  = CompanyTelefonNumber;
        company.EmailAddress = CompanyEmailAddress;
        company.BankName = CompanyBankName;
        company.Iban = CompanyIban;
        company.Bic = CompanyBic;
        company.Ustidnr = CompanyUstidnr;
        company.Stnr = CompanyStnr;
        company.Logo = CompanyLogo;
        
        return company;
    }
}
