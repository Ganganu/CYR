using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Core;
using CYR.Dialog;
using CYR.Invoice;
using CYR.Model;
using CYR.OrderItems;
using CYR.Services;
using CYR.Settings;
using CYR.UnitOfMeasure;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class CreateInvoiceViewModel : ObservableObject, IParameterReceiver
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
        private readonly ISaveInvoiceInvoicePositionService _saveInvoiceInvoicePositionService;
        private readonly IPreviewInvoiceService _previewInvoiceService;
        private int _positionCounter = 1;
        private Client _client;

        public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository,
            IUnitOfMeasureRepository unitOfMeasureRepository,
            INavigationService navigationService,
            ISaveInvoiceInvoicePositionService saveInvoiceInvoicePositionService,
            IPreviewInvoiceService previewInvoiceService)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            NavigationService = navigationService;
            _saveInvoiceInvoicePositionService = saveInvoiceInvoicePositionService;
            _previewInvoiceService = previewInvoiceService;
            InvoiceDate = DateTime.Now;
            Initialize();
        }
        private void Initialize()
        {
            Positions = new ObservableCollection<InvoicePosition> { new InvoicePosition(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString() } };
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }
        [ObservableProperty]
        private string _clientId;
        [ObservableProperty]
        private string _clientName;
        [ObservableProperty]
        private string _clientStreet;
        [ObservableProperty]
        private string _clientCityPlz;
        [ObservableProperty]
        private int _invoiceNumber;
        [ObservableProperty]
        private string _issueDate;
        [ObservableProperty]
        private string _dueDate;
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
        private DateTime _invoiceDate;
        [ObservableProperty]
        private DateTime _startDate;
        [ObservableProperty]
        private DateTime _endDate;
        public INavigationService NavigationService { get; }
        partial void OnInvoiceNumberChanged(int value)
        {
            InvoiceDocumentDataSource.SetInvoiceNumber(value);
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
    }
}