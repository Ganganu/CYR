using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Invoice.InvoiceModels;
using CYR.OrderItems;
using CYR.UnitOfMeasure;
using System.Collections.ObjectModel;

namespace CYR.Invoice.InvoiceViewModels;

public partial class InvoicePosition : ValidationViewModelBase
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

    public InvoicePosition(IOrderItemRepository orderItemRepository, IUnitOfMeasureRepository unitOfMeasureRepository)
    {
        _orderItemRepository = orderItemRepository;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        WeakReferenceMessenger.Default.Register<OrderItemMessageCollectionChanged>(this, (r, m) =>
        {
            ItemColectionChanged(m);
        });
        Initialize();
    }
    public InvoicePosition()
    {
        isInDatabaseButManuallyChanged = false;
        isInDatabaseButManuallyChangedValueAlreadySet = false;
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
    public OrderItem? _orderItem;
    [ObservableProperty]
    public string? _manuallyInsertedArticle;
    partial void OnManuallyInsertedArticleChanged(string? oldValue, string? newValue)
    {
        if (oldValue != newValue)
        {
            if (OrderItem is null)
            {
                OrderItem = new OrderItem();
                OrderItem.Name = newValue;
                OrderItem.Description = newValue;
                OrderItem.Price = 0;
            }
        }
    }
    private bool isInDatabaseButManuallyChanged;
    private bool isInDatabaseButManuallyChangedValueAlreadySet;
    partial void OnOrderItemChanged(OrderItem? oldValue, OrderItem? newValue)
    {
        if (isInDatabaseButManuallyChangedValueAlreadySet) return;
        if (newValue?.Description is not null && newValue.Id == 0) isInDatabaseButManuallyChanged = true;
        if (isInDatabaseButManuallyChanged)
        {
            Price = newValue?.Price;
            TotalPrice = Convert.ToDecimal(Quantity) * Price;
            isInDatabaseButManuallyChangedValueAlreadySet = true;
            return;
        }
        if (oldValue != newValue)
        {
            if (OrderItem is null) return;
            if (OrderItem.Description is null) return;
            if (string.IsNullOrEmpty(OrderItem.Description)) return;
            Price = OrderItem.Price;
            TotalPrice = Convert.ToDecimal(Quantity) * Price;
        }
    }

    [ObservableProperty]
    private string? _quantity;
    partial void OnQuantityChanged(string? oldValue, string? newValue)
    {
        if (!ValidateQuantity(newValue)) return;
        if (oldValue != newValue)
        {
            if (OrderItem != null)
            {
                Price = OrderItem.Price;
                TotalPrice = Convert.ToDecimal(Quantity) * Price;
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
            TotalPrice = Convert.ToDecimal(Quantity) * Price;
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
        if (newValue is null) return;
        if (oldValue != newValue)
        {
            Messenger.Send(new InvoiceTotalPriceEvent(newValue));
        }
    }

    [ObservableProperty]
    private ObservableCollection<OrderItem>? _items;

    private async Task<IEnumerable<OrderItem>> GetAllItems()
    {
        return await _orderItemRepository.GetAllAsync();
    }
    [ObservableProperty]
    private ObservableCollection<UnitOfMeasureModel>? _unitsOfMeasure;
    private async Task<IEnumerable<UnitOfMeasureModel>> GetAllUnitOfMeasures()
    {
        return await _unitOfMeasureRepository.GetAllAsync();
    }

    /// <summary>
    /// Gesendet von CreateNewArticleViewModel, damit die Items geupdated werden
    /// </summary>
    /// <param name="orderItemMessageCollectionChanged"></param>
    private async void ItemColectionChanged(OrderItemMessageCollectionChanged orderItemMessageCollectionChanged)
    {
        if (orderItemMessageCollectionChanged.Value)
        {
            var latestItems = await GetAllItems();
            Items = [.. latestItems];
        }
    }
    #region Validations
    private bool ValidateQuantity(string? value)
    {
        bool succes = false;
        ClearErrors(nameof(Quantity));
        if (string.IsNullOrEmpty(value)) return succes;
        if (string.IsNullOrWhiteSpace(value)) return succes;
        if (!decimal.TryParse(value, out _))
            AddError(nameof(Quantity), "Menge muss eine Zahl sein");
        else
            succes = true;
        return succes;
    }
    #endregion

}
