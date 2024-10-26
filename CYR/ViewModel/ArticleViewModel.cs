using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Clients;
using CYR.OrderItems;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class ArticleViewModel : ObservableObject
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public ArticleViewModel(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
            Initialize();
        }

        private async void Initialize()
        {
            IEnumerable<OrderItem.OrderItem> orderItems = await _orderItemRepository.GetAllAsync();
            OrderItems = new ObservableCollection<OrderItem.OrderItem>(orderItems);
        }

        [ObservableProperty]
        private ObservableCollection<OrderItem.OrderItem>? _orderItems;



    }
}
