using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Logging;
using CYR.Messages;
using CYR.Services;
using CYR.User;

namespace CYR.UnitOfMeasure;

public partial class UpdateUnitOfMeasureViewModel : ObservableRecipientWithValidation, IParameterReceiver
{
    private readonly UserContext _userContext;
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
    private readonly LoggingRepository _loggingRepository;
    public UpdateUnitOfMeasureViewModel(IUnitOfMeasureRepository unitOfMeasureRepository, UserContext userContext, LoggingRepository loggingRepository, INavigationService navigation)
    {
        _unitOfMeasureRepository = unitOfMeasureRepository;
        _userContext = userContext;
        _loggingRepository = loggingRepository;
        _navigation = navigation;
    }


    [ObservableProperty]
    private INavigationService _navigation;


    [ObservableProperty]
    private string? _id;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _name;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string? _description;

    [RelayCommand]
    private void NavigateBack()
    {
        Navigation.NavigateTo<UnitOfMeasureViewModel>();
    }

    [RelayCommand]
    private async Task UpdateUnitOfMeasure()
    {
        ValidateAllProperties();
        if (HasErrors) return;
        UnitOfMeasureModel unitOfMeasureModel = new UnitOfMeasureModel();
        unitOfMeasureModel.Id = Convert.ToInt32(Id);
        unitOfMeasureModel.Name = Name;
        unitOfMeasureModel.Description = Description;

        await _unitOfMeasureRepository.UpdateAsync(unitOfMeasureModel);
        await _loggingRepository.InsertAsync(CreateHisModel(unitOfMeasureModel));
        Messenger.Send(new SnackbarMessage($"Die Einheit {unitOfMeasureModel.Name} wurde erfolgreich aktualisiert.", "Check"));
    }

    private HisModel CreateHisModel(UnitOfMeasureModel unitOfMeasureModel)
    {
        HisModel model = new HisModel();
        model.LoggingType = LoggingType.UnitOfMeasureUpdated;
        model.UserId = _userContext.CurrentUser.Id;
        model.UnitOfMeasureId = unitOfMeasureModel.Id;
        model.Message = $@"Einheit: {unitOfMeasureModel.Name} wurde vom User: {_userContext.CurrentUser.Id} aktualisiert.";
        return model;
    }

    public async Task ReceiveParameter(object parameter)
    {
        UnitOfMeasureModel unitOfMeasureModel = (UnitOfMeasureModel)parameter;
        if (unitOfMeasureModel is null) return;
        Id = unitOfMeasureModel.Id.ToString();
        Name = unitOfMeasureModel.Name;
        Description = unitOfMeasureModel.Description;
    }
}
