using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Settings;
using CYR.User;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardUserViewModel : ObservableRecipient
{
    private readonly ISelectImageService _selectImageService;
    private readonly UserRepository _userRepository;
    private readonly UserContext _userContext;
    public DashboardUserViewModel(ISelectImageService selectImageService, UserRepository userRepository, UserContext userContext)
    {
        _selectImageService = selectImageService;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    [ObservableProperty]
    private User.User? _user;
    [ObservableProperty]
    private Company _company;
    [ObservableProperty]
    private string? _userLogoSource;
    public string? UserLogo => string.IsNullOrEmpty(_userLogoSource) ? @"/Ressources/user.png" : UserLogoSource;


}
