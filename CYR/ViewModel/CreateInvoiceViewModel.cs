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
        private readonly IConfigurationService _configurationService;
        private int _positionCounter = 1;
        private Client _client;
        private InvoiceModel _invoiceModel;
        private string? _dialogResponse;
        public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository, IUnitOfMeasureRepository unitOfMeasureRepository,
            IInvoiceRepository invoiceRepository, IInvoicePositionRepository invoicePositionRepository,
            IDatabaseConnection databaseConnection, IDialogService dialogService,INavigationService navigationService,
            IConfigurationService configurationService)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            _invoiceRepository = invoiceRepository;
            _invoicePositionRepository = invoicePositionRepository;
            _databaseConnection = databaseConnection;
            _dialogService = dialogService;
            NavigationService = navigationService;
            _configurationService = configurationService;
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
        private void CreateInvoice()
        {
            IEnumerable<InvoicePosition> positions = Positions;
            UserSettings userSettings = _configurationService.GetUserSettings();
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(_client, positions, _invoiceModel, userSettings);
            var document = new InvoiceDocument(model);
            document.GeneratePdfAndShow();
        }
        [RelayCommand]
        private async Task SaveInvoice()
        {
            if (Positions is null)
            {
                return;
            }
            if (Positions.Count <= 0)
            {
                return;
            }
            if (Positions.Any(p => p.Price < 0))
            {
                ShowErrorDialog("Fehler", "Der Preis eines ausgewählten Artikels ist kleiner 0.",
                                "Abbrechen",
                                "Warning",
                                Visibility.Collapsed, "");
                return;
            }
            if (StartDate > EndDate)
            {
                ShowErrorDialog("Fehler", "Das Startdatum ist größer als Enddatum!",
                                "Abbrechen",
                                "Warning",
                                Visibility.Collapsed, "");
                return;
            }
            bool checkPositionNull = Positions.Any(p => p.OrderItem == null || p.Quantity <= 0 || p.OrderItem.Name == null);
            if (checkPositionNull)
            {
                ShowErrorDialog("Fehler", "Die ausgewählten Artikel enthalten Problemen!",
                                "Abbrechen",
                                "Warning",
                                Visibility.Collapsed, "");
                return;
            }
            using (var connection = new SQLiteConnection(_databaseConnection.ConnectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (_client == null)
                        {
                            ShowErrorDialog("Warnung", "Wählen Sie einen Kunden bevor Sie eine Rechnung schreiben. Möchten Sie zum Kundentab navigieren", 
                                "Nein", 
                                "Warning", 
                                Visibility.Visible, "Ja");
                            if (_dialogResponse == "True")
                            {
                                NavigationService.NavigateTo<ClientViewModel>();                                
                            }
                            return;
                        }
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
                            NetAmount = Positions.Sum(x => x.Price * x.Quantity),
                            Paragraph = "13b",
                            State = InvoiceState.Open,
                            Subject = Subject,
                            ObjectNumber = ObjectNumber,
                            Mwst = IsMwstApplicable,
                            StartDate = StartDate.ToShortDateString(),
                            EndDate = EndDate.ToShortDateString()
                        };
                        if (IsMwstApplicable)
                        {
                            invoiceModel.GrossAmount = Math.Round(invoiceModel.NetAmount * 1.19m,2);
                        }
                        else
                        {
                            invoiceModel.GrossAmount = invoiceModel.NetAmount;                                                        
                        }
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
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        ShowErrorDialog("Fehler", $"Die Rechnung mit der Rechnungsnummer {InvoiceNumber} existiert bereits!" +
                        $"Ändern Sie die Rechnungsnummer und versuchen Sie es erneut.", "Abbrechen", "Error", Visibility.Collapsed,
                        "");
                        return;
                    }
                    catch(Exception ex)
                    {
                        throw;
                    }
                }
            }
            IEnumerable<InvoicePosition> positions = Positions;
            UserSettings userSettings = _configurationService.GetUserSettings();
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(_client, positions, _invoiceModel, userSettings);
            //model.Seller = _user;
            model.Subject = Subject;
            model.ObjectNumber = ObjectNumber;
            model.Mwst = IsMwstApplicable;
            model.StartDate = StartDate.ToShortDateString();
            model.EndDate = EndDate.ToShortDateString();
            var document = new InvoiceDocument(model);
            document.GeneratePdfAndShow();
        }

        private void ShowErrorDialog(string title,
            string message,
            string cancelButtonText,
            string icon, 
            Visibility okButtonVisibility, string okButtonText)
        {
            _dialogService.ShowDialog<ErrorDialogViewModel>(result =>
            {
                _dialogResponse = result;
            },
            new Dictionary<Expression<Func<ErrorDialogViewModel, object>>, object>
            {
                { vm => vm.Title, title },
                { vm => vm.Message,  message},
                { vm => vm.CancelButtonText, cancelButtonText },
                { vm => vm.Icon,icon },
                { vm => vm.OkButtonText, okButtonText },
                { vm => vm.IsOkVisible, okButtonVisibility}
            });
        }
    }
}