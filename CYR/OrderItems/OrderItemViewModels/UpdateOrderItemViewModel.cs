using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Core;
using CYR.Logging;
using CYR.Services;
using CYR.User;
using CYR.ViewModel;

namespace CYR.OrderItems.OrderItemViewModels;

public partial class UpdateOrderItemViewModel : ObservableRecipientWithValidation, IParameterReceiver
{
    private IEnumerable<OrderItem>? _articles;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly LoggingRepository _loggingRepository;    
    private readonly UserContext _userContext;

    public UpdateOrderItemViewModel(INavigationService navigationService, IOrderItemRepository orderItemRepository, UserContext userContext, LoggingRepository loggingRepository)
    {
        _orderItemRepository = orderItemRepository;
        Navigation = navigationService;
        _userContext = userContext;
        _loggingRepository = loggingRepository;
    }
    [ObservableProperty]
    private string? _updateMessage;
    [ObservableProperty]
    private int? _id;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string _name = string.Empty;

    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    [RegularExpression("^(?:\\d{0,9}\\,\\d{1,2})$|^\\d{1,2}$", ErrorMessage = "Nur Zahlen dürfen eingegeben werden.")]
    private string? _price;
    [ObservableProperty]
    private INavigationService _navigation;

    [RelayCommand]
    private void NavigateBack()
    {
        if (_articles != null)
        {
            Navigation.NavigateTo<ArticleViewModel>(_articles);
        }
        else
        {
            Navigation.NavigateTo<ArticleViewModel>();
        }
    }

    [RelayCommand]
    private async Task UpdateOrderItem()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        OrderItem orderItemToUpdate = new OrderItem();
        orderItemToUpdate.Id = Id.Value;
        orderItemToUpdate.Name = Name;
        orderItemToUpdate.Description = Description;
        orderItemToUpdate.Price = Price;

        try
        {
            await _orderItemRepository.UpdateAsync(orderItemToUpdate);

        }
        catch (Exception)
        {
            throw;
        }
        UpdateMessage = $"Artikel/Dienstleistung {orderItemToUpdate.Name} erfolgreich aktualisiert.";
        await _loggingRepository.InsertAsync(CreateHisModel(orderItemToUpdate));
    }

    private HisModel CreateHisModel(OrderItem orderItemToUpdate)
    {
        HisModel model = new HisModel();
        model.LoggingType = LoggingType.OrderItemUpdated;
        model.UserId = _userContext.CurrentUser.Id;
        model.OrderItemId = orderItemToUpdate.Id.ToString();
        model.Message = $@"Artikel: {orderItemToUpdate.Description} wurder erfolgreich vom User: {_userContext.CurrentUser.Id} geupdatet.";
        return model;
    }

    /// <summary>
    /// Wenn von ArticleView navigiert wird.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public async Task ReceiveParameter(object parameter)
    {
        if (parameter is null) return;
        OrderItem item = (OrderItem)parameter;   
        Id = item.Id;
        Name = item.Name;
        Description = item.Description;
        Price = item.Price.ToString();
    }
}
