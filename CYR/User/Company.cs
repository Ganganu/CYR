using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.User;

public partial class Company : ObservableRecipient
{
    public string? Id { get; set; }
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _street;
    [ObservableProperty]
    private string? _city;
    [ObservableProperty]
    private string? _plz;
    [ObservableProperty]
    private string? _houseNumber;
    [ObservableProperty]
    private string? _telefonNumber;
    [ObservableProperty]
    private string? _emailAddress;
    [ObservableProperty]
    private string? _bankName;
    [ObservableProperty]
    private string? _iban;
    [ObservableProperty]
    private string? _bic;
    [ObservableProperty]
    private string? _ustidnr;
    [ObservableProperty]
    private string? _stnr;
    [ObservableProperty]
    private string? _logo;
}
