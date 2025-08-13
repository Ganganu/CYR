using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Logging;
using CYR.Services;
using CYR.User;
using CYR.ViewModel;

namespace CYR.OrderItems.OrderItemViewModels;

public partial class UpdateOrderItemViewModel : ObservableRecipient, IParameterReceiver
{
    private IEnumerable<OrderItem> _articles;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly LoggingRepository _loggingRepository;    
    private readonly UserContext _userContext;

    public UpdateOrderItemViewModel(INavigationService navigationService, IOrderItemRepository orderItemRepository, UserContext userContext)
    {
        _orderItemRepository = orderItemRepository;
        Navigation = navigationService;
        _userContext = userContext;
    }
    [ObservableProperty]
    private string? _updateMessage;
    [ObservableProperty]
    private OrderItem _orderItem;
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
        if (OrderItem is null) return;
        OrderItem orderItemToUpdate = CreateNewOrderItem(OrderItem);

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

    private OrderItem CreateNewOrderItem(OrderItem orderItem)
    {
        OrderItem oi = new OrderItem();
        oi.Id = orderItem.Id;
        oi.Name = orderItem.Name;
        oi.Description = orderItem.Description;
        oi.Price = orderItem.Price;
        return oi;
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
        OrderItem = item;
    }
}
