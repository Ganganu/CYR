using CYR.Clients;
using CYR.Dialog;
using CYR.Invoice.Model;
using CYR.Invoice.Repository;
using CYR.Model;
using CYR.Services;
using CYR.Settings;
using CYR.ViewModel;
using QuestPDF.Fluent;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.Invoice.Service
{
    public class PreviewInvoiceService : IPreviewInvoiceService
    {
        private readonly IInvoiceDocument _invoiceDocument;
        private readonly IDialogService _dialogService;
        private readonly IConfigurationService _configurationService;
        private InvoiceModel? _invoiceModel;
        private string? _dialogResponse;
        public PreviewInvoiceService(INavigationService navigationService, IInvoiceDocument invoiceDocument,
            IDialogService dialogService, IConfigurationService configurationService)
        {
            NavigationService = navigationService;
            _invoiceDocument = invoiceDocument;
            _dialogService = dialogService;
            _configurationService = configurationService;
        }
        public INavigationService NavigationService { get; }
        public async Task SaveInvoice(CreateInvoiceModel createInvoiceModel)
        {
            if (createInvoiceModel.Positions is null)
            {
                return;
            }
            if (createInvoiceModel.Positions.Count <= 0)
            {
                return;
            }
            if (createInvoiceModel.Positions.Any(p => p.Price < 0))
            {
                ShowErrorDialog("Fehler", "Der Preis eines ausgewählten Artikels ist kleiner 0.",
                                "Abbrechen",
                                "Warning",
                                Visibility.Collapsed, "", createInvoiceModel);
                return;
            }
            if (createInvoiceModel.StartDate > createInvoiceModel.EndDate)
            {
                ShowErrorDialog("Fehler", "Das Startdatum ist größer als das Enddatum!",
                                "Abbrechen",
                                "Warning",
                                Visibility.Collapsed, "", createInvoiceModel);
                return;
            }
            bool checkPositionNull = createInvoiceModel.Positions.Any(p => p.OrderItem == null || p.Quantity <= 0 || p.OrderItem.Name == null);
            if (checkPositionNull)
            {
                ShowErrorDialog("Fehler", "Die ausgewählten Artikel enthalten Problemen!",
                "Abbrechen",
                "Warning",
                Visibility.Collapsed, "", createInvoiceModel);
                return;
            }

            if (createInvoiceModel.Client == null)
            {
                ShowErrorDialog("Warnung", "Wählen Sie einen Kunden bevor Sie eine Rechnung schreiben. Möchten Sie zum Kundentab navigieren",
                    "Nein",
                    "Warning",
                    Visibility.Visible, "Ja", createInvoiceModel);
                if (_dialogResponse == "True")
                {
                    NavigationService.NavigateTo<ClientViewModel>();
                }
                return;
            }

            Client client = new Client
            {
                ClientNumber = createInvoiceModel.Client.ClientNumber
            };

            if (createInvoiceModel.StartDate.HasValue)
            {

            }

            InvoiceModel invoiceModel = new InvoiceModel();
            invoiceModel.InvoiceNumber = createInvoiceModel.InvoiceNumber;
            invoiceModel.Customer = client;
            invoiceModel.IssueDate = createInvoiceModel.InvoiceDate.Value.ToShortDateString();
            invoiceModel.DueDate = DateTime.Now.ToShortDateString();
            invoiceModel.NetAmount = createInvoiceModel?.Positions.Sum(x => x.Price * x.Quantity);
            invoiceModel.Paragraph = "13b";
            invoiceModel.State = InvoiceState.Open;
            invoiceModel.Subject = createInvoiceModel.Subject;
            invoiceModel.ObjectNumber = createInvoiceModel.ObjectNumber;
            invoiceModel.Mwst = createInvoiceModel.IsMwstApplicable;
            if (createInvoiceModel.StartDate.HasValue)
                invoiceModel.StartDate = createInvoiceModel.StartDate.Value.ToShortDateString();
            if (createInvoiceModel.EndDate.HasValue)
                invoiceModel.EndDate = createInvoiceModel.EndDate.Value.ToShortDateString();

            if (createInvoiceModel.IsMwstApplicable)
            {
                invoiceModel.GrossAmount = Math.Round((decimal)invoiceModel.NetAmount * 1.19m, 2);
            }
            else
            {
                invoiceModel.GrossAmount = invoiceModel.NetAmount;
            }
            _invoiceModel = invoiceModel;

            foreach (var position in createInvoiceModel.Positions)
            {
                InvoicePositionModel ipm;
                if (position.OrderItem is not null && position.OrderItem.Id == 0)
                {
                    OrderItem.OrderItem manuallyInsertedOrderItem = ManuallyInsertedItemToOrderItem(position);
                    ipm = CreateInvoicePositionModel(manuallyInsertedOrderItem, position, invoiceModel);
                    position.OrderItem.Name = manuallyInsertedOrderItem.Name;
                    position.OrderItem.Description = manuallyInsertedOrderItem.Description;
                }
                else
                {
                    ipm = CreateInvoicePositionModel(position.OrderItem, position, invoiceModel);
                }
                //await _invoicePositionRepository.InsertAsync(ipm, null);
            }

            CreateInvoice(createInvoiceModel);
        }

        private void CreateInvoice(CreateInvoiceModel createInvoiceModel)
        {
            IEnumerable<InvoicePosition> positions = createInvoiceModel.Positions;
            UserSettings userSettings = _configurationService.GetUserSettings();
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(createInvoiceModel.Client, positions, _invoiceModel, userSettings);
            model.Subject = createInvoiceModel.Subject;
            model.ObjectNumber = createInvoiceModel.ObjectNumber;
            model.Mwst = createInvoiceModel.IsMwstApplicable;
            model.Notiz = createInvoiceModel.Notiz;
            if (createInvoiceModel.StartDate.HasValue)
                model.StartDate = createInvoiceModel.StartDate.Value.ToShortDateString();
            if (createInvoiceModel.EndDate.HasValue)
                model.EndDate = createInvoiceModel.EndDate.Value.ToShortDateString();
            _invoiceDocument.Model = model;
            _invoiceDocument.GeneratePdfAndShow();

        }

        private static InvoicePositionModel CreateInvoicePositionModel(OrderItem.OrderItem orderItem, InvoicePosition position, InvoiceModel invoiceModel)
        {
            var invoicePositionModel = new InvoicePositionModel
            {
                InvoiceNumber = invoiceModel.InvoiceNumber.ToString(),
                Description = orderItem.Description,
                Quantity = position.Quantity,
                UnitOfMeasure = position.UnitOfMeasure.Name,
                UnitPrice = orderItem.Price,
                TotalPrice = position.TotalPrice
            };
            return invoicePositionModel;
        }

        private static OrderItem.OrderItem ManuallyInsertedItemToOrderItem(InvoicePosition invoicePosition)
        {
            OrderItem.OrderItem item = new OrderItem.OrderItem();
            item.Name = invoicePosition.ManuallyInsertedArticle;
            item.Price = invoicePosition.Price;
            item.Description = invoicePosition.ManuallyInsertedArticle;
            return item;
        }

        private void ShowErrorDialog(string title,
            string message,
            string cancelButtonText,
            string icon,
            Visibility okButtonVisibility, string okButtonText, CreateInvoiceModel createInvoiceModel)
        {
            _dialogService.ShowDialog(result =>
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