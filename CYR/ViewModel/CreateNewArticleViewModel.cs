using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Logging;
using CYR.OrderItems;
using CYR.Services;
using CYR.User;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace CYR.ViewModel
{
    public partial class CreateNewArticleViewModel : ObservableObject
    {
        private IEnumerable<OrderItem> _articles;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly LoggingRepository _loggingRepository;
        private readonly UserContext _userContext;
        public CreateNewArticleViewModel(INavigationService navigationService, IOrderItemRepository orderItemRepository, LoggingRepository loggingRepository, UserContext userContext)
        {
            _orderItemRepository = orderItemRepository;
            Navigation = navigationService;
            _loggingRepository = loggingRepository;
            _userContext = userContext;
        }

        [ObservableProperty]
        private string? _saveMessage;
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private decimal? _price;
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
            await _loggingRepository.InsertAsync(CreateHisModel(orderItem));
        }

        private HisModel CreateHisModel(OrderItem orderItem)
        {
            HisModel model = new HisModel();
            model.LoggingType = LoggingType.OrderItemCreated;
            model.UserId = _userContext.CurrentUser.Id;
            model.OrderItemId = orderItem.Id.ToString();
            model.Message = $@"Artikel: {orderItem.Description} wurder vom User: {_userContext.CurrentUser.Id} erstellt.";
            return model;
        }
    }
}
