using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Settings;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardUserViewModel : ObservableRecipient
{
    private readonly ISelectImageService _selectImageService;
    public DashboardUserViewModel(ISelectImageService selectImageService)
    {
        _selectImageService = selectImageService;
    }

    [ObservableProperty]
    private string? _userLogo;

    [RelayCommand]
    private void ChangeAvatorFoto()
    {
        var image = _selectImageService.SelectStringImage();
        UserLogo = image;
    }

}
