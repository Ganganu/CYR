using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Invoice.InvoiceModels;
using CYR.OrderItems;
using CYR.UnitOfMeasure;
using System.Collections.ObjectModel;

namespace CYR.Invoice.InvoiceViewModels
{
    public partial class InvoicePosition : ObservableRecipient
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

        public InvoicePosition(IOrderItemRepository orderItemRepository, IUnitOfMeasureRepository unitOfMeasureRepository) 
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            WeakReferenceMessenger.Default.Register<OrderItemMessageCollectionChanged>(this, (r,m) =>
            {
                ItemColectionChanged(m);
            });
            Initialize();
        }
        public InvoicePosition()
        {            
        }

        private async void Initialize()
        {
            Items = [.. await GetAllItems()];
            UnitsOfMeasure = [.. await GetAllUnitOfMeasures()];
        }

        [ObservableProperty]
        private bool _isInvoicePositionSelected;
        [ObservableProperty]
        private string? _id;
        [ObservableProperty]
        private string? _invoiceNumber;
        [ObservableProperty]
        public OrderItem.OrderItem? _orderItem;
        [ObservableProperty]
        public string? _manuallyInsertedArticle;
        partial void OnManuallyInsertedArticleChanged(string? oldValue, string? newValue)
        {
            if (oldValue != newValue)
            {
                if (OrderItem is null)
                {
                    OrderItem = new OrderItem.OrderItem();
                    OrderItem.Name = newValue;
                    OrderItem.Description = newValue;
                    OrderItem.Price = 0;
                }
            }
        }
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
        private decimal? _quantity;
        partial void OnQuantityChanged(decimal? oldValue, decimal? newValue)
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
        private decimal? _price;
        partial void OnPriceChanged(decimal? oldValue, decimal? newValue)
        {
            if (oldValue != newValue)
            {
                TotalPrice = Quantity * Price;
                if (OrderItem is not null)
                {
                    OrderItem.Price = Price;                    
                }
            }
        }

        [ObservableProperty]
        public decimal? _totalPrice;

        partial void OnTotalPriceChanged(decimal? oldValue, decimal? newValue)
        {
            if (oldValue != newValue)
            {
                Messenger.Send(new InvoiceTotalPriceEvent(newValue));
            }
        }

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
        private async void ItemColectionChanged(OrderItemMessageCollectionChanged orderItemMessageCollectionChanged)
        {
            if (orderItemMessageCollectionChanged.Value)
            {
                var latestItems = await GetAllItems();
                Items = [.. latestItems];
            }
        }
    }
}
