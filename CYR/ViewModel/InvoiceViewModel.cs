using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Invoice;
using CYR.Model;
using CYR.OrderItems;
using CYR.Services;
using CYR.UnitOfMeasure;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class InvoiceViewModel : ObservableObject, IParameterReceiver
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
        private int _positionCounter = 1;
        private Client _client;
        public InvoiceViewModel(IOrderItemRepository orderItemRepository, IUnitOfMeasureRepository unitOfMeasureRepository) 
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            Initialize();
        }
        private void Initialize()
        {
            Positions = new ObservableCollection<InvoicePosition> { new InvoicePosition(_orderItemRepository,_unitOfMeasureRepository) {Id = _positionCounter.ToString() } };
        }
        [ObservableProperty]
        private string _clientName;
        [ObservableProperty]
        private string _clientStreet;
        [ObservableProperty]
        private string _clientCityPlz;
        [ObservableProperty]
        private string _user;
        [ObservableProperty]
        private string _userStreet;
        [ObservableProperty]
        private string _userCityPlz;
        [ObservableProperty]
        private int _invoiceNumber;
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
            Positions?.Add(new InvoicePosition(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString()});            
        }
        [RelayCommand]
        private void DeleteRow(object parameter)
        {
            InvoicePosition invoicePosition = (InvoicePosition)parameter;
            Positions?.Remove(invoicePosition);
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
        public void CreateInvoice()
        {
            IEnumerable<InvoicePosition> positions = Positions;
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(_client, positions);
            var document = new InvoiceDocument(model);
            document.GeneratePdfAndShow();
        }
    }
}
