using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.OrderItems;
using CYR.Services;
using CommunityToolkit.Mvvm.Messaging;

namespace CYR.ViewModel
{
    public partial class CreateNewArticleViewModel : ObservableObject
    {
        private IEnumerable<OrderItem> _articles;
        private readonly IOrderItemRepository _orderItemRepository;
        public CreateNewArticleViewModel(INavigationService navigationService, IOrderItemRepository orderItemRepository) 
        {
            _orderItemRepository = orderItemRepository;
            Navigation = navigationService;
        }

        [ObservableProperty]
        private string? _saveMessage;
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
                Navigation.NavigateTo<ArticleViewModel>(_articles);
            }
            else
            {
                Navigation.NavigateTo<ArticleViewModel>();
            }
        }

        [RelayCommand]
        private async Task SaveArticle()
        {
            OrderItem orderItem = new OrderItem();
            orderItem.Name = Name;
            orderItem.Description = Description;
            orderItem.Price = Price;
            await _orderItemRepository.InsertAsync(orderItem);
            _articles = await _orderItemRepository.GetAllAsync();
            if (_articles != null)
            {
                WeakReferenceMessenger.Default.Send(new OrderItemMessageCollectionChanged(true));
            }
            SaveMessage = $"Artikel/Dienstleistung {orderItem.Name} erfolgreich gespeichert.";
        }
    }
}
