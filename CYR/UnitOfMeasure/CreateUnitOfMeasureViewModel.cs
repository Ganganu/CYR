using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Logging;
using CYR.Messages;
using CYR.Services;
using CYR.User;

namespace CYR.UnitOfMeasure;

public partial class CreateUnitOfMeasureViewModel : ObservableRecipientWithValidation
{
    private readonly UserContext _userContext;
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
    private readonly LoggingRepository _loggingRepository;
    public CreateUnitOfMeasureViewModel(UserContext userContext, IUnitOfMeasureRepository unitOfMeasureRepository, LoggingRepository loggingRepository, INavigationService navigation)
    {
        _userContext = userContext;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _loggingRepository = loggingRepository;
        _navigation = navigation;
    }

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _name;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _description;

    [RelayCommand]
    private async Task SaveUnitOfMeasure()
    {
        ValidateAllProperties();
        if (HasErrors) return;
        UnitOfMeasureModel unitOfMeasureModel = new UnitOfMeasureModel();
        unitOfMeasureModel.Name = Name;
        unitOfMeasureModel.Description = Description;
        await _unitOfMeasureRepository.InsertAsync(unitOfMeasureModel);
        await _loggingRepository.InsertAsync(CreateHisModel(unitOfMeasureModel));
        Messenger.Send(new SnackbarMessage($"Die Einheit {unitOfMeasureModel.Name} wurde erfolgreich gespeichert.", "Check"));
    }


    [RelayCommand]
    private void NavigateBack()
    {
        Navigation.NavigateTo<UnitOfMeasureViewModel>();
    }
    private HisModel CreateHisModel(UnitOfMeasureModel unitOfMeasureModel)
    {
        HisModel model = new HisModel();
        model.LoggingType = LoggingType.UnitOfMeasureCreated;
        model.UserId = _userContext.CurrentUser.Id;
        model.UnitOfMeasureId = unitOfMeasureModel.Id;
        model.Message = $@"Einheit: {unitOfMeasureModel.Name} wurde vom User: {_userContext.CurrentUser.Id} erstellt.";
        return model;
    }
}
