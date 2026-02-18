using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Logging;
using CYR.Messages;
using CYR.OrderItems;
using CYR.Services;
using CYR.User;

namespace CYR.ViewModel
{
    public partial class CreateNewArticleViewModel : ObservableRecipientWithValidation
    {
        private IEnumerable<OrderItem>? _articles;
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
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Feld darf nicht leer sein.")]
        private string _name = string.Empty;

        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Feld darf nicht leer sein.")]
        [ObservableProperty]
        private string? _description;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Feld darf nicht leer sein.")]
        [RegularExpression(@"^(?:\d{0,9}[.,]\d{1,2})$|^\d{1,2}$", ErrorMessage = "Nur Zahlen dürfen eingegeben werden.")]
        private double? _price;
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
            ValidateAllProperties();
            if (HasErrors) return;
            OrderItem orderItem = new OrderItem();
            orderItem.Name = Name;
            orderItem.Description = Description;
            //if (Price is not null && Price.Contains(',')) Price = Price.Replace(',', '.');
            orderItem.Price = Price;
            await _orderItemRepository.InsertAsync(orderItem);
            _articles = await _orderItemRepository.GetAllAsync();
            if (_articles != null)
            {
                WeakReferenceMessenger.Default.Send(new OrderItemMessageCollectionChanged(true));
            }
            bool result = await _loggingRepository.InsertAsync(CreateHisModel(orderItem));
            if (result) NavigateBack();
            Messenger.Send(new SnackbarMessage($"Artikel/Dienstleistung {orderItem.Name} wurde erfolgreich gespeichert.", "Check"));
        }

        private HisModel CreateHisModel(OrderItem orderItem)
        {
            HisModel model = new HisModel();
            model.LoggingType = LoggingType.OrderItemCreated;
            model.UserId = _userContext.CurrentUser.Id;
            model.OrderItemId = orderItem.Id.ToString();
            model.Message = $@"Artikel: {orderItem.Description} wurde vom User: {_userContext.CurrentUser.Id} erstellt.";
            return model;
        }
    }
}
