using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Core;
using CYR.Services;
using CYR.ViewModel;
using System.Data.SQLite;
using System.Windows;

namespace CYR.OrderItems.OrderItemViewModels;

public partial class UpdateOrderItemViewModel : ObservableRecipient, IParameterReceiver
{
    private IEnumerable<OrderItem> _articles;
    private readonly IOrderItemRepository _orderItemRepository;

    public UpdateOrderItemViewModel(INavigationService navigationService, IOrderItemRepository orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;
        Navigation = navigationService;       
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
