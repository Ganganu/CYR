using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Clients;
using CYR.Invoice;
using CYR.Invoice.Model;
using CYR.Invoice.Repository;
using CYR.Invoice.Service;
using CYR.Model;
using CYR.OrderItem;
using CYR.OrderItems;
using CYR.Services;
using CYR.Settings;
using CYR.UnitOfMeasure;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace CYR.ViewModel
{
    public partial class CreateInvoiceViewModel : ObservableRecipient, IParameterReceiver, IRecipient<LogoEvent>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
        private readonly ISaveInvoiceInvoicePositionService _saveInvoiceInvoicePositionService;
        private readonly IPreviewInvoiceService _previewInvoiceService;
        private readonly IRetrieveClients _retrieveClients;
        private readonly IConfigurationService _configurationService;
        private readonly UserSettings _userSettings;
        private int _positionCounter = 1;
        private Client? _client;

        public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository,
            IUnitOfMeasureRepository unitOfMeasureRepository,
            INavigationService navigationService,
            ISaveInvoiceInvoicePositionService saveInvoiceInvoicePositionService,
            IPreviewInvoiceService previewInvoiceService,
            IRetrieveClients retrieveClients, 
            IConfigurationService configurationService)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            NavigationService = navigationService;
            _saveInvoiceInvoicePositionService = saveInvoiceInvoicePositionService;
            _previewInvoiceService = previewInvoiceService;
            _retrieveClients = retrieveClients;
            _configurationService = configurationService;
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

        [ObservableProperty]
        private ObservableCollection<InvoicePosition>? _positions;

        [RelayCommand]
        private void AddNewRow()
        {
            _positionCounter++;
            Positions?.Add(new InvoicePosition(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString() });
        }
        [RelayCommand]
        private void DeleteRow(object parameter)
        {
            InvoicePosition invoicePosition = (InvoicePosition)parameter;
            Positions?.Remove(invoicePosition);
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
        public void ReceiveParameter(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            Client clientParameter = parameter as Client;
            _client = clientParameter;
            if (clientParameter != null)
            {
                //ClientName = clientParameter.Name;
                //ClientStreet = clientParameter.Street;
                //ClientCityPlz = $"{clientParameter.City} {clientParameter.PLZ}";
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
        private void DeleteInvoicePosition()
        {
            var selectedPositions = Positions?.Where(p => p.IsInvoicePositionSelected).ToList();
            if (selectedPositions is null)  return;
            foreach (var position in selectedPositions)
            {
                Positions?.Remove(position);
            }
        }
        [RelayCommand]
        private void OpenImageInDefaultApp()
        {
            string imagePath = $@"C:\GGA\Kleine_Projekte\IGF.png";
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = imagePath,
                        UseShellExecute = true // Ensures the default image viewer is used
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening image: {ex.Message}");
                }
            }
        }

        public void Receive(LogoEvent message)
        {
            Logo = message.Logo;
        }
    }
}