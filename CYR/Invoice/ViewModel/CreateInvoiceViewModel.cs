using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Invoice;
using CYR.Invoice.Model;
using CYR.Invoice.Repository;
using CYR.Invoice.Service;
using CYR.Model;
using CYR.OrderItems;
using CYR.Services;
using CYR.UnitOfMeasure;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class CreateInvoiceViewModel : ObservableObject, IParameterReceiver
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
        private readonly ISaveInvoiceInvoicePositionService _saveInvoiceInvoicePositionService;
        private readonly IPreviewInvoiceService _previewInvoiceService;
        private readonly IRetrieveClients _retrieveClients;
        private int _positionCounter = 1;
        private Client? _client;

        public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository,
            IUnitOfMeasureRepository unitOfMeasureRepository,
            INavigationService navigationService,
            ISaveInvoiceInvoicePositionService saveInvoiceInvoicePositionService,
            IPreviewInvoiceService previewInvoiceService,
            IRetrieveClients retrieveClients)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            NavigationService = navigationService;
            _saveInvoiceInvoicePositionService = saveInvoiceInvoicePositionService;
            _previewInvoiceService = previewInvoiceService;
            _retrieveClients = retrieveClients;
            Initialize();
        }
        private async void Initialize()
        {
            Positions = new ObservableCollection<InvoicePosition> { new(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString() } };
            IEnumerable<Client> cl = await _retrieveClients.Handle();
            Clients = [.. cl];
        }
        [ObservableProperty]
        private string? _clientId;
        [ObservableProperty]
        private string? _clientName;
        [ObservableProperty]
        private string? _clientStreet;
        [ObservableProperty]
        private string? _clientCityPlz;
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
            if (parameters is null)
            {
                return;
            }
            InvoicePosition invoicePosition = (InvoicePosition)parameters;
            OrderItem.OrderItem orderItem = new();
            orderItem.Name = invoicePosition.ManuallyInsertedArticle;
            orderItem.Description = invoicePosition.ManuallyInsertedArticle;
            orderItem.Price = invoicePosition.Price;
            await _orderItemRepository.InsertAsync(orderItem);
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
                ClientName = clientParameter.Name;
                ClientStreet = clientParameter.Street;
                ClientCityPlz = $"{clientParameter.City} {clientParameter.PLZ}";
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
    }
}