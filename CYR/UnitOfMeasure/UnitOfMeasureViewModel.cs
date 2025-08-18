using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Logging;
using CYR.OrderItems.OrderItemViewModels;
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
    [RelayCommand]
    private void UpdateUnitOfMeasure()
    {
        if (UnitsOfMeasure is null) return;
        var itemToUpdate = UnitsOfMeasure.Where(o => o.IsSelected == true).FirstOrDefault();
        if (itemToUpdate is null) return;
        Navigation.NavigateTo<UpdateUnitOfMeasureViewModel>(itemToUpdate);
    }
    [RelayCommand]
    private async  Task DeleteUnitOfMeasure()
    {
        if (UnitsOfMeasure is null) return;
        var itemsToDelete = UnitsOfMeasure.Where(o => o.IsSelected == true).ToList();
        if (itemsToDelete.Count < 1) return;
        try
        {            
            foreach (var item in itemsToDelete)
            {
                try
                {
                    var c = await _unitOfMeasureRepository.DeleteAsync(item);
                    await _loggingRepository.InsertAsync(CreateHisModel(item));
                    UnitsOfMeasure.Remove(item);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    private HisModel CreateHisModel(UnitOfMeasureModel item)
    {
        HisModel model = new HisModel();
        model.LoggingType = LoggingType.UnitOfMeasureDeleted;
        model.UserId = _userContext.CurrentUser.Id;
        model.UnitOfMeasureId = item.Id;
        model.Message = $@"Einheit: {item.Name} wurde vom User: {_userContext.CurrentUser.Id} gelöscht.";
        return model;
    }
}
