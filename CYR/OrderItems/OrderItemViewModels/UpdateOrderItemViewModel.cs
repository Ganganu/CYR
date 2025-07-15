using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Services;
using CYR.ViewModel;

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
