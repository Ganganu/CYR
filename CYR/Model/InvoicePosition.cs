using CommunityToolkit.Mvvm.ComponentModel;
using CYR.OrderItems;
using System.Collections.ObjectModel;

namespace CYR.Model
{
    public partial class InvoicePosition : ObservableObject
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public InvoicePosition(IOrderItemRepository orderItemRepository) 
        {
            _orderItemRepository = orderItemRepository;
            Initialize();
        }

        private async void Initialize()
        {
            Items = new ObservableCollection<OrderItem.OrderItem>(await GetAllItems());
        }

        public string? Id { get; set; }
        public string? InvoiceNumber { get; set; }
        [ObservableProperty]
        public OrderItem.OrderItem? _orderItem;
        partial void OnOrderItemChanged(OrderItem.OrderItem? oldValue, OrderItem.OrderItem? newValue)
        {
            if (oldValue != newValue)
            {
                Price = OrderItem.Price;
                TotalPrice = Quantity * Price;
            }
        }

        [ObservableProperty]
        public int _quantity;
        partial void OnQuantityChanged(int oldValue, int newValue)
        {
            if (oldValue != newValue)
            {
                Price = OrderItem.Price;
                TotalPrice = Quantity * Price;
            }
        }

        [ObservableProperty]
        public string? _unitOfMeasure;

        [ObservableProperty]
        private decimal _price;
        partial void OnPriceChanged(decimal oldValue, decimal newValue)
        {
            if (oldValue != newValue)
            {
                TotalPrice = Quantity * Price;
            }
        }

        [ObservableProperty]
        public decimal _totalPrice;
        [ObservableProperty]
        private ObservableCollection<OrderItem.OrderItem>? _items;

        private async Task<IEnumerable<OrderItem.OrderItem>> GetAllItems()
        {
            return await _orderItemRepository.GetAllAsync();
        }
    }
}
