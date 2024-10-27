using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.OrderItems;
using CYR.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYR.ViewModel
{
    public partial class CreateNewArticleViewModel : ObservableObject
    {
        private IEnumerable<Client> _articles;
        private readonly IOrderItemRepository _orderItemRepository;
        public CreateNewArticleViewModel(INavigationService navigationService, IOrderItemRepository orderItemRepository) 
        {
            _orderItemRepository = orderItemRepository;
            Navigation = navigationService;
        }

        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private decimal _price;
        [ObservableProperty]
        private INavigationService _navigation;

        [RelayCommand]
        private void NavigateBack()
        {
            if (_articles != null)
            {
                Navigation.NavigateTo<ClientViewModel>(_articles);
            }
        }

        [RelayCommand]
        private void SaveArticle()
        {
            OrderItem.OrderItem orderItem = new OrderItem.OrderItem();
            orderItem.Name = Name;
            orderItem.Description = Description;
            orderItem.Price = Price;
            _orderItemRepository.InsertAsync(orderItem);
        }
    }
}
