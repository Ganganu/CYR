using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Core;
using CYR.OrderItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Items = new ObservableCollection<OrderItem.OrderItem>(await GetAllItems());
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
        private ObservableCollection<OrderItem.OrderItem> _items;

        private async Task<IEnumerable<OrderItem.OrderItem>> GetAllItems()
        {
            return await _orderItemRepository.GetAllAsync();
        }
    }
}
