using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Model;
using CYR.OrderItems;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public InvoiceViewModel(IOrderItemRepository orderItemRepository) 
        {
            this._orderItemRepository = orderItemRepository;
            Initialize();
        }
        private async void Initialize()
        {
            Items = new List<OrderItem.OrderItem>(await GetAllItems());
            Positions = new ObservableCollection<InvoicePosition> { new InvoicePosition() };
        }
        [ObservableProperty]
        private string? _id;
        [ObservableProperty]
        private int _amount;
        [ObservableProperty]
        private string? _unitOfMeasure;
        [ObservableProperty]
        private string? _description;
        [ObservableProperty]
        private double _unitPrice;
        [ObservableProperty]
        private double _totalPrice;
        [ObservableProperty]
        private ObservableCollection<InvoicePosition> _positions;
        [ObservableProperty]
        private List<OrderItem.OrderItem> _items;

        private async Task<IEnumerable<OrderItem.OrderItem>> GetAllItems()
        {
            return await _orderItemRepository.GetAllAsync();
        }
    }
}
