using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Clients;
using CYR.Invoice;
using CYR.Invoice.Model;
using CYR.Invoice.Repository;
using CYR.Invoice.Service;
using CYR.Model;
using CYR.OrderItems;
using CYR.Services;
using CYR.Settings;
using CYR.UnitOfMeasure;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CYR.ViewModel
{
    public partial class CreateInvoiceViewModel : ObservableRecipient, IRecipient<LogoEvent>, IRecipient<InvoiceTotalPriceEvent>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
        private readonly ISaveInvoiceInvoicePositionService _saveInvoiceInvoicePositionService;
        private readonly IPreviewInvoiceService _previewInvoiceService;
        private readonly IRetrieveClients _retrieveClients;
        private readonly IConfigurationService _configurationService;
        private readonly IOpenImageService _openImageService;
        private readonly UserSettings _userSettings;

        private int _positionCounter = 1;
        private Client? _client;

        public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository,
            IUnitOfMeasureRepository unitOfMeasureRepository,
            INavigationService navigationService,
            ISaveInvoiceInvoicePositionService saveInvoiceInvoicePositionService,
            IPreviewInvoiceService previewInvoiceService,
            IRetrieveClients retrieveClients, 
            IConfigurationService configurationService,
            IOpenImageService openImageService)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            NavigationService = navigationService;
            _saveInvoiceInvoicePositionService = saveInvoiceInvoicePositionService;
            _previewInvoiceService = previewInvoiceService;
            _retrieveClients = retrieveClients;
            _configurationService = configurationService;
            _openImageService = openImageService;
            _userSettings = _configurationService.GetUserSettings();
            Initialize();
            Messenger.RegisterAll(this);
        }
        private async void Initialize()
        {
            Positions = new ObservableCollection<InvoicePosition> { new(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString() } };
            IEnumerable<Client> cl = await _retrieveClients.Handle();
            Clients = [.. cl];
            Logo = _userSettings.Logo;
        }
        [ObservableProperty]
        private string? _notiz;
        [ObservableProperty]
        private int? _invoiceNumber;
        [ObservableProperty]
        private string? _issueDate;
        [ObservableProperty]
        private string? _dueDate;
        [ObservableProperty]
        private decimal? _netAmount;
        [ObservableProperty]
        private decimal? _grossAmount;
        [ObservableProperty]
        private string? _subject;
        [ObservableProperty]
        private string? _objectNumber;
        [ObservableProperty]
        private bool _isMwstApplicable;
        [ObservableProperty]
        private DateTime? _invoiceDate;
        [ObservableProperty]
        private DateTime? _startDate;
        [ObservableProperty]
        private DateTime? _endDate;
        [ObservableProperty]
        private ObservableCollection<Client> _clients;
        [ObservableProperty]
        private Client? _selectedClient;
        [ObservableProperty]
        private ImageSource _logo;
        [ObservableProperty]
        private decimal? _totalPrice = 0.0m;
        public INavigationService NavigationService { get; }
        partial void OnInvoiceNumberChanged(int? value)
        {
            InvoiceDocumentDataSource.SetInvoiceNumber(value);
        }
        partial void OnSelectedClientChanged(Client? oldValue, Client? newValue)      
        {
            if (newValue != oldValue)
            {
                _client = newValue;
            }
        }

        partial void OnIsMwstApplicableChanged(bool value)
        {
            if (value == true) TotalPrice *= 1.19m;
            else TotalPrice /= 1.19m;
        }

        [ObservableProperty]
        private ObservableCollection<InvoicePosition>? _positions;

        [RelayCommand]
        private void AddNewRow()
        {
            _positionCounter++;
            Positions?.Add(new InvoicePosition(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString() });
        }
        [RelayCommand]
        private async Task SaveArticle(object parameters)
        {
            var selectedPositions = Positions?.Where(p => p.IsInvoicePositionSelected).ToList();
            if (selectedPositions is null) return;
            foreach (var position in selectedPositions)
            {
                OrderItem.OrderItem orderItem = new();
                orderItem.Name = position.ManuallyInsertedArticle;
                orderItem.Description = position.ManuallyInsertedArticle;
                orderItem.Price = position.Price;
                await _orderItemRepository.InsertAsync(orderItem);
            }
        }

        [RelayCommand]
        private async Task PreviewInvoice()
        {
            CreateInvoiceModel createInvoiceModel = new()
            {
                Client = _client,
                EndDate = EndDate,
                InvoiceDate = InvoiceDate,
                InvoiceNumber = InvoiceNumber,
                IsMwstApplicable = IsMwstApplicable,
                ObjectNumber = ObjectNumber,
                Positions = Positions,
                Notiz = Notiz,
                StartDate = StartDate,
                Subject = Subject
            };
            await _previewInvoiceService.SaveInvoice(createInvoiceModel);
        }
        [RelayCommand]
        private async Task SaveInvoice()
        {
            CreateInvoiceModel createInvoiceModel = new()
            {
                Client = _client,
                EndDate = EndDate,
                InvoiceDate = InvoiceDate,
                InvoiceNumber = InvoiceNumber,
                IsMwstApplicable = IsMwstApplicable,
                ObjectNumber = ObjectNumber,
                Positions = Positions,
                Notiz = Notiz,
                StartDate = StartDate,
                Subject = Subject
            };
            await _saveInvoiceInvoicePositionService.SaveInvoice(createInvoiceModel);
        }
        [RelayCommand]
        private void PrintInvoice()
        {

        }
        [RelayCommand]
        private void DeleteInvoicePosition()
        {
            var selectedPositions = Positions?.Where(p => p.IsInvoicePositionSelected).ToList();
            if (selectedPositions is null)  return;
            foreach (var position in selectedPositions)
            {
                Positions?.Remove(position);
                _positionCounter--;
            }
        }
        [RelayCommand]
        private void OpenImageInDefaultApp()
        {
            string imagePath = new Uri(Logo.ToString()).LocalPath;      
            _openImageService.OpenImage(imagePath);
        }

        public void Receive(LogoEvent message)
        {
            Logo = message.Logo;
        }

        public void Receive(InvoiceTotalPriceEvent message)
        {
            TotalPrice = 0;
            foreach (var item in Positions)
            {
                TotalPrice += item.TotalPrice;
            }
        }
    }
}