using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Core;
using CYR.Logging;
using CYR.User;

namespace CYR.UnitOfMeasure;

public partial class CreateUnitOfMeasureViewModel : ObservableRecipientWithValidation
{
    private UserContext _userContext;
    private IUnitOfMeasureRepository _unitOfMeasureRepository;
    private LoggingRepository _loggingRepository;
    public CreateUnitOfMeasureViewModel(UserContext userContext, IUnitOfMeasureRepository unitOfMeasureRepository, LoggingRepository loggingRepository)
    {
        _userContext = userContext;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _loggingRepository = loggingRepository;
    }

    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _description;

    [RelayCommand]
    private void SaveUnitOfMeasure()
    {

    }
}
