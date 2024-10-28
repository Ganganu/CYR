using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.OrderItems;
using CYR.Services;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class ArticleViewModel : ObservableObject, IParameterReceiver
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public ArticleViewModel(IOrderItemRepository orderItemRepository, INavigationService navigationService)
        {
            _orderItemRepository = orderItemRepository;
            Navigation = navigationService;
            Initialize();
        }

        private async void Initialize()
        {
            IEnumerable<OrderItem.OrderItem> orderItems = await _orderItemRepository.GetAllAsync();
            OrderItems = new ObservableCollection<OrderItem.OrderItem>(orderItems);
        }

        [ObservableProperty]
        private INavigationService _navigation;
        [ObservableProperty]
        private ObservableCollection<OrderItem.OrderItem>? _orderItems;

        [RelayCommand]
        private void AddNewRow()
        {
            Navigation.NavigateTo<CreateNewArticleViewModel>();
        }
        [RelayCommand]
        private async Task DeleteRow(object parameter)
        {
            OrderItem.OrderItem orderItem = (OrderItem.OrderItem)parameter;
            await _orderItemRepository.DeleteAsync(orderItem);
            var items = await _orderItemRepository.GetAllAsync();
            OrderItems = new ObservableCollection<OrderItem.OrderItem>(items);
        }

        public void ReceiveParameter(object parameter)
        {
            if (parameter != null)
            {
                ObservableCollection<OrderItem.OrderItem> oi = new ObservableCollection<OrderItem.OrderItem>((List<OrderItem.OrderItem>)parameter);
                if (oi != null)
                {
                    OrderItems = oi;
                }
            }
        }
    }
}
