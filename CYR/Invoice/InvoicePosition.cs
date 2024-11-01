using CommunityToolkit.Mvvm.ComponentModel;
using CYR.OrderItems;
using CYR.UnitOfMeasure;
using System.Collections.ObjectModel;

namespace CYR.Model
{
    public partial class InvoicePosition : ObservableObject
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

        public InvoicePosition(IOrderItemRepository orderItemRepository, IUnitOfMeasureRepository unitOfMeasureRepository) 
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            Initialize();
        }

        private async void Initialize()
        {
            Items = new ObservableCollection<OrderItem.OrderItem>(await GetAllItems());
            UnitsOfMeasure = new ObservableCollection<UnitOfMeasureModel>(await GetAllUnitOfMeasures());
        }

        [ObservableProperty]
        private string? _id;
        [ObservableProperty]
        private string? _invoiceNumber;
        [ObservableProperty]
        public OrderItem.OrderItem? _orderItem;
        partial void OnOrderItemChanged(OrderItem.OrderItem? oldValue, OrderItem.OrderItem? newValue)
        {
            if (oldValue != newValue)
            {
                if (OrderItem != null)
                {
                    Price = OrderItem.Price;
                    TotalPrice = Quantity * Price;
                }
            }
        }

        [ObservableProperty]
        private decimal _quantity;
        partial void OnQuantityChanged(decimal oldValue, decimal newValue)
        {
            if (oldValue != newValue)
            {
                if (OrderItem != null)
                {
                    Price = OrderItem.Price;
                    TotalPrice = Quantity * Price;
                }
            }
        }
        
        [ObservableProperty]
        private UnitOfMeasureModel? _unitOfMeasure;

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
        [ObservableProperty]
        private ObservableCollection<UnitOfMeasureModel>? _unitsOfMeasure;
        private async Task<IEnumerable<UnitOfMeasureModel>> GetAllUnitOfMeasures()
        {
            return await _unitOfMeasureRepository.GetAllAsync();
        }
    }
}
