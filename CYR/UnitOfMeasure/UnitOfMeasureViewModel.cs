using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Logging;
using CYR.Services;
using CYR.User;
using CYR.ViewModel;

namespace CYR.UnitOfMeasure;

public partial class UnitOfMeasureViewModel : ObservableRecipientWithValidation
{
    private readonly LoggingRepository _loggingRepository;
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
    private readonly UserContext _userContext;
    public UnitOfMeasureViewModel(LoggingRepository loggingRepository, UserContext userContext, IUnitOfMeasureRepository unitOfMeasureRepository, INavigationService navigation)
    {
        _loggingRepository = loggingRepository;
        _userContext = userContext;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        Initialize();
        _navigation = navigation;
    }

    private async Task Initialize()
    {
        var uoms = await _unitOfMeasureRepository.GetAllAsync();
        UnitsOfMeasure = [.. uoms];
    }

    [ObservableProperty]
    private ObservableCollection<UnitOfMeasureModel> _unitsOfMeasure;

    [ObservableProperty]
    private INavigationService _navigation;


    [RelayCommand]
    private void NavigateBack()
    {
        Messenger.Send(new NavigateBackSource(typeof(UnitOfMeasureViewModel)));
    }
    [RelayCommand]
    private void NavigateToCreateUnitOfMeasure()
    {
        Navigation.NavigateTo<CreateUnitOfMeasureViewModel>();
    }
}
