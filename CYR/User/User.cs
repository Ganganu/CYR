using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.User;

public partial class User : ObservableRecipient
{   
    public string? Id { get; set; }
    [ObservableProperty]
    private string? _username;
    [ObservableProperty]
    private string? _password;
    [ObservableProperty]
    private string? _role;
    [ObservableProperty]
    private string? _logo;
}
