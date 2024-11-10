using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Core;
using CYR.Dialog;
using CYR.Invoice;
using CYR.Model;
using CYR.OrderItems;
using CYR.Services;
using CYR.UnitOfMeasure;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.ViewModel
{
    public partial class CreateInvoiceViewModel : ObservableObject, IParameterReceiver
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoicePositionRepository _invoicePositionRepository;
        private readonly IDatabaseConnection _databaseConnection;
        private readonly IDialogService _dialogService;
        private int _positionCounter = 1;
        private Client _client;
        private InvoiceModel _invoiceModel;
        public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository, IUnitOfMeasureRepository unitOfMeasureRepository,
            IInvoiceRepository invoiceRepository, IInvoicePositionRepository invoicePositionRepository,
            IDatabaseConnection databaseConnection, IDialogService dialogService) 
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            _invoiceRepository = invoiceRepository;
            _invoicePositionRepository = invoicePositionRepository;
            _databaseConnection = databaseConnection;
            _dialogService = dialogService;
            InvoiceDate = DateTime.Now;
            Initialize();
        }
        private void Initialize()
        {
            Positions = new ObservableCollection<InvoicePosition> { new InvoicePosition(_orderItemRepository,_unitOfMeasureRepository) {Id = _positionCounter.ToString() } };
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
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(_client, positions, _invoiceModel);
            var document = new InvoiceDocument(model);
            document.GeneratePdfAndShow();
        }
        [RelayCommand]
        public async Task SaveInvoice()
        {
            using (var connection = new SQLiteConnection(_databaseConnection.ConnectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Create and populate invoice
                        Client client = new Client
                        {
                            ClientNumber = _client.ClientNumber
                        };

                         InvoiceModel invoiceModel = new InvoiceModel
                        {
                            InvoiceNumber = InvoiceNumber,
                            Customer = client,
                            IssueDate = InvoiceDate.ToShortDateString(),
                            DueDate = DateTime.Now.ToShortDateString(),
                            NetAmount = Positions.Sum(x => x.Price),
                            Paragraph = "13b",
                            State = InvoiceState.Open,
                            Subject = Subject,
                            ObjectNumber = ObjectNumber,
                            Mwst = IsMwstApplicable
                        };
                        invoiceModel.GrossAmount = invoiceModel.NetAmount;
                        _invoiceModel = invoiceModel;

                        // Save invoice
                        await _invoiceRepository.InsertAsync(invoiceModel, transaction);

                        // Save each position
                        foreach (var position in Positions)
                        {
                            var invoicePositionModel = new InvoicePositionModel
                            {
                                InvoiceNumber = invoiceModel.InvoiceNumber.ToString(),
                                Description = position.OrderItem.Description,
                                Quantity = position.Quantity,
                                UnitOfMeasure = position.UnitOfMeasure.Name,
                                UnitPrice = position.Price,
                                TotalPrice = position.TotalPrice
                            };
                            await _invoicePositionRepository.InsertAsync(invoicePositionModel, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        _dialogService.ShowDialog<ErrorDialogViewModel>(result =>
                        {
                            var response = result;
                        },
                        new Dictionary<Expression<Func<ErrorDialogViewModel, object>>, object>
                        {
                            { vm => vm.Title, "Error" },
                            { vm => vm.Message, $"Die Rechnung mit der Rechnungsnummer {InvoiceNumber} existiert bereits!" +
                            $"Ändern Sie die Rechnungsnummer und versuchen es erneut." },
                            { vm => vm.CancelButtonText, "Abbrechen" },
                            { vm => vm.Icon,"Error" },
                            { vm => vm.IsOkVisible, Visibility.Collapsed}
                        });
                        return;
                    }
                }
            }
            IEnumerable<InvoicePosition> positions = Positions;
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(_client, positions, _invoiceModel);
            model.Subject = Subject;
            model.ObjectNumber = ObjectNumber;
            model.Mwst = IsMwstApplicable;
            var document = new InvoiceDocument(model);
            document.GeneratePdfAndShow();
        }
    }
}