using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Dialog;
using CYR.Logging;
using CYR.OrderItems;
using CYR.OrderItems.OrderItemViewModels;
using CYR.Services;
using CYR.User;

namespace CYR.ViewModel;

public partial class ArticleViewModel : ObservableRecipient, IParameterReceiver
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IDialogService _dialogService;
    private readonly IPrintOrderItemService _printOrderItemService;
    private readonly LoggingRepository _loggingRepository; 
    private readonly UserContext _userContext;

    private string? _dialogResponse;
    public ArticleViewModel(INavigationService navigationService, IOrderItemRepository orderItemRepository, IDialogService dialogService, IPrintOrderItemService printOrderItemService, LoggingRepository loggingRepository, UserContext userContext)
    {
        _orderItemRepository = orderItemRepository;
        Navigation = navigationService;
        Initialize();
        _dialogService = dialogService;
        _printOrderItemService = printOrderItemService;
        _loggingRepository = loggingRepository;
        _userContext = userContext;
    }

    private async void Initialize()
    {
        IEnumerable<OrderItem> orderItems = await _orderItemRepository.GetAllAsync();
        OrderItems = new ObservableCollection<OrderItem>(orderItems);
    }

    [ObservableProperty]
    private INavigationService _navigation;

    [ObservableProperty]
    private ObservableCollection<OrderItem>? _orderItems;

    [RelayCommand]
    private void NavigateBack()
    {
        Messenger.Send(new NavigateBackSource(typeof(ClientViewModel)));
    }

    [RelayCommand]
    private void NavigateToCreateNewArticle()
    {
        Navigation.NavigateTo<CreateNewArticleViewModel>();
    }


    [RelayCommand]
    private async Task DeleteArticle()
    {
        if (OrderItems is null) return;
        var itemsToDelete = OrderItems.Where(o => o.IsSelected == true).ToList();
        if (itemsToDelete.Count < 1) return;
        try
        {
            ShowNotificationDialog("Artikel/Dienstleistungen löschen.", $"Möchten Sie wirklich die ausgewählten Artikel/Dienstaleistungen löschen?",
                    "Nein", "Item", Visibility.Visible, "Ja");
            if (_dialogResponse != "True")
            {
                return;
            }
            foreach (var item in itemsToDelete)
            {
                try
                {
                    var c = await _orderItemRepository.DeleteAsync(item);
                    OrderItems.Remove(item);
                    await _loggingRepository.InsertAsync(CreateHisModel(item));
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

    private HisModel CreateHisModel(OrderItem item)
    {
        HisModel model = new HisModel();
        model.LoggingType = LoggingType.OrderItemDeleted;
        model.OrderItemId = item.Id.ToString();
        model.UserId = _userContext.CurrentUser.Id;
        model.Message = $@"Artikel: {item.Description} wurder vom User: {_userContext.CurrentUser.Id} gelöscht.";
        return model;
    }

    [RelayCommand]
    private void UpdateOrderItem()
    {
        if (OrderItems is null) return;
        var itemToUpdate = OrderItems.Where(o => o.IsSelected == true).FirstOrDefault();
        if (itemToUpdate is null) return;
        Navigation.NavigateTo<UpdateOrderItemViewModel>(itemToUpdate);
    }

    public async Task ReceiveParameter(object parameter)
    {
        if (parameter != null)
        {
            ObservableCollection<OrderItem> oi = new ObservableCollection<OrderItem>((List<OrderItem>)parameter);
            if (oi != null)
            {
                OrderItems = oi;
            }
        }
    }

    private void ShowNotificationDialog(string title,
        string message,
        string cancelButtonText,
        string icon,
        Visibility okButtonVisibility, string okButtonText)
    {
        _dialogService.ShowDialog(result =>
        {
            _dialogResponse = result;
        },
        new Dictionary<Expression<Func<NotificationViewModel, object>>, object>
        {
            { vm => vm.Title, title },
            { vm => vm.Message,  message},
            { vm => vm.CancelButtonText, cancelButtonText },
            { vm => vm.Icon,icon },
            { vm => vm.OkButtonText, okButtonText },
            { vm => vm.IsOkVisible, okButtonVisibility}
        });
    }

    [RelayCommand]
    private void InsertOrderItems()
    {

    }
    [RelayCommand]
    private void PrintOrderItems()
    {
        if (OrderItems is null) return;
        _printOrderItemService.Print(OrderItems);
    }
}