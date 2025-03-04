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
        private readonly IInvoicePositionRepository _invoicePositionRepository;
        private readonly IDialogService _dialogService;
        private readonly IConfigurationService _configurationService;
        private InvoiceModel _invoiceModel;
        private string? _dialogResponse;
        public PreviewInvoiceService(INavigationService navigationService, IInvoicePositionRepository invoicePositionRepository,
            IDialogService dialogService, IConfigurationService configurationService)
        {
            NavigationService = navigationService;
            _invoicePositionRepository = invoicePositionRepository;
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

            InvoiceModel invoiceModel = new InvoiceModel
            {
                InvoiceNumber = createInvoiceModel.InvoiceNumber,
                Customer = client,
                IssueDate = createInvoiceModel.InvoiceDate.ToShortDateString(),
                DueDate = DateTime.Now.ToShortDateString(),
                NetAmount = createInvoiceModel.Positions.Sum(x => x.Price * x.Quantity),
                Paragraph = "13b",
                State = InvoiceState.Open,
                Subject = createInvoiceModel.Subject,
                ObjectNumber = createInvoiceModel.ObjectNumber,
                Mwst = createInvoiceModel.IsMwstApplicable,
                StartDate = createInvoiceModel.StartDate.ToShortDateString(),
                EndDate = createInvoiceModel.EndDate.ToShortDateString()
            };
            if (createInvoiceModel.IsMwstApplicable)
            {
                invoiceModel.GrossAmount = Math.Round(invoiceModel.NetAmount * 1.19m, 2);
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
                await _invoicePositionRepository.InsertAsync(ipm, null);
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
            model.StartDate = createInvoiceModel.StartDate.ToShortDateString();
            model.EndDate = createInvoiceModel.EndDate.ToShortDateString();
            var document = new InvoiceDocument(model);
            document.GeneratePdfAndShow();
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