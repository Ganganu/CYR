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
        public ArticleViewModel(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
            Initialize();
        }

        private async void Initialize()
        {
            IEnumerable<OrderItem> orderItems = await _orderItemRepository.GetAllAsync();
            OrderItems = new ObservableCollection<OrderItem>(orderItems);
        }

        [ObservableProperty]
        private ObservableCollection<OrderItem>? _orderItems;

        [RelayCommand]
        private async Task DeleteRow(object parameter)
        {
            OrderItem orderItem = (OrderItem)parameter;
            await _orderItemRepository.DeleteAsync(orderItem);
            var items = await _orderItemRepository.GetAllAsync();
            OrderItems = new ObservableCollection<OrderItem>(items);
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
    }
}
