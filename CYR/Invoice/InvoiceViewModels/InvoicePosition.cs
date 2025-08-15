using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Invoice.InvoiceModels;
using CYR.OrderItems;
using CYR.UnitOfMeasure;

namespace CYR.Invoice.InvoiceViewModels;

public partial class InvoicePosition : ObservableRecipientWithValidation
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
                OrderItem.Price = "0";
            }
        }
    }

    private bool isInDatabaseButManuallyChanged;
    private bool isInDatabaseButManuallyChangedValueAlreadySet;
    /// <summary>
    /// Der User kann der Preis für ein OrderItem ändern. Wenn die Eigenschaft aktualisiert wird, wird diese Methode aufgerufen.
    /// Ich aktualisiere den Preis, wenn der User einen OrderItem ausgewählt hat. Wenn er aber den Preis ändert und die Rechnung speichert und 
    /// dann abruft, wird diese Method 3 Mal aufgerufen. 1 - wenn die TextBox erzeugt wird (leer). 2 - Die Daten aus der Datenbank (die gespeicherte in Rechnung)
    /// 3 - weil der OrderItem in der Datenbank ist, wird der Preis mit der Daten von der Datenbank überschrieben (die Daten für OrderItem - Tabelle Produkte_Dienstleistungen).
    /// Ich versuche in dieser Methode den 3. Abruf zu verhindern, wenn der Preis geändert wurde.
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    partial void OnOrderItemChanged(OrderItem? oldValue, OrderItem? newValue)
    {
        if (isInDatabaseButManuallyChangedValueAlreadySet) return;
        if (newValue?.Description is not null && newValue.Id == 0) isInDatabaseButManuallyChanged = true;
        if (isInDatabaseButManuallyChanged)
        {
            Price = newValue?.Price;
            TotalPrice = Convert.ToDecimal(Quantity) * Convert.ToDecimal(Price);
            isInDatabaseButManuallyChangedValueAlreadySet = true;
            return;
        }
        if (oldValue != newValue)
        {
            if (OrderItem is null) return;
            if (OrderItem.Description is null) return;
            if (string.IsNullOrEmpty(OrderItem.Description)) return;
            Price = OrderItem.Price;
            TotalPrice = Convert.ToDecimal(Quantity) * Convert.ToDecimal(Price);
        }
    }


    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    [RegularExpression("^(?:\\d{1,9})(?:,\\d{1,2})?$", ErrorMessage = "Nur Zahlen dürfen eingegeben werden.")]
    private string? _quantity;
    partial void OnQuantityChanged(string? oldValue, string? newValue)
    {
        if (oldValue != newValue)
        {
            if (OrderItem != null)
            {
                Price = OrderItem.Price;
                TotalPrice = Convert.ToDecimal(Quantity) * Convert.ToDecimal(Price);
            }
        }
    }

    [ObservableProperty]
    private UnitOfMeasureModel? _unitOfMeasure;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    [RegularExpression("^(?:\\d{1,9})(?:,\\d{1,2})?$", ErrorMessage = "Nur Zahlen dürfen eingegeben werden.")]
    private string? _price;
    partial void OnPriceChanged(string? oldValue, string? newValue)
    {
        if (oldValue != newValue)
        {
            TotalPrice = Convert.ToDecimal(Quantity) * Convert.ToDecimal(Price);
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
}
