using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Services;
using CYR.Settings;
using System.Windows.Media;

namespace CYR.ViewModel;

public partial class SettingsViewModel : ObservableRecipient
{
    //private readonly IConfigurationService _configurationService;
    //private readonly ISelectImageService _selectImageService;
    //private readonly UserSettings _userSettings;

    //public SettingsViewModel(IConfigurationService configurationService, ISelectImageService selectImageService)
    //{
    //    _configurationService = configurationService;
    //    _selectImageService = selectImageService;
    //    _userSettings = _configurationService.GetUserSettings();

    //    LoadSettings();
    //}

    //private void LoadSettings()
    //{
    //    Name = _userSettings.Name;
    //    Street = _userSettings.Street;
    //    City = _userSettings.City;
    //    Plz = _userSettings.PLZ;
    //    HouseNumber = _userSettings.HouseNumber;
    //    Telefonnumber = _userSettings.Telefonnumber;
    //    EmailAddress = _userSettings.EmailAddress;
    //    BankName = _userSettings.BankName;
    //    Iban = _userSettings.IBAN;
    //    Bic = _userSettings.BIC;
    //    Ustidnr = _userSettings.USTIDNR;
    //    Stnr = _userSettings.STNR;
    //    Logo = _userSettings.Logo;
    //}

    //[ObservableProperty]
    //private Color _themeColor;

    //[ObservableProperty]
    //private string _name = string.Empty;

    //[ObservableProperty]
    //private string _street = string.Empty;

    //[ObservableProperty]
    //private string _city = string.Empty;
    //[ObservableProperty]
    //private string? _plz;

    //[ObservableProperty]
    //private string _houseNumber = string.Empty;

    //[ObservableProperty]
    //private string _telefonnumber = string.Empty;

    //[ObservableProperty]
    //private string _emailAddress = string.Empty;

    //[ObservableProperty]
    //private string _bankName = string.Empty;

    //[ObservableProperty]
    //private string _iban = string.Empty;

    //[ObservableProperty]
    //private string _bic = string.Empty;

    //[ObservableProperty]
    //private string _ustidnr = string.Empty;

    //[ObservableProperty]
    //private string _stnr = string.Empty;

    //[ObservableProperty]
    //private ImageSource _logo;

    //[RelayCommand]
    //private void SaveSettings()
    //{
    //    _userSettings.Name = Name ?? string.Empty;
    //    _userSettings.Street = Street ?? string.Empty;
    //    _userSettings.City = City ?? string.Empty;
    //    _userSettings.PLZ = Plz ?? string.Empty;
    //    _userSettings.HouseNumber = HouseNumber ?? string.Empty;
    //    _userSettings.Telefonnumber = Telefonnumber ?? string.Empty;
    //    _userSettings.EmailAddress = EmailAddress ?? string.Empty;
    //    _userSettings.BankName = BankName ?? string.Empty;
    //    _userSettings.IBAN = Iban ?? string.Empty;
    //    _userSettings.BIC = Bic ?? string.Empty;
    //    _userSettings.USTIDNR = Ustidnr ?? string.Empty;
    //    _userSettings.STNR = Stnr ?? string.Empty;
    //    _userSettings.Logo = Logo;

    //    _configurationService.SaveSettings();
    //    Messenger.Send(new LogoEvent(Logo));
    //}
    //[RelayCommand]
    //private void SelectImage()
    //{
    //    var message = _selectImageService.SelectImage();
    //    Logo = message.ImageSource;
    //    Messenger.Send(message.Message, message.Icon);
    //}
}