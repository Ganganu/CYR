using CYR.Clients;
using CYR.Dialog;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceViewModels;
using CYR.Services;
using CYR.Settings;
using QuestPDF.Fluent;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.Invoice.InvoiceServices
{
    public class PreviewInvoiceService : IPreviewInvoiceService
    {
        private readonly IInvoiceDocument _invoiceDocument;
        private readonly IDialogService _dialogService;
        private readonly IConfigurationService _configurationService;
        private InvoiceModel? _invoiceModel;
        private string? _dialogResponse;
        public PreviewInvoiceService(IInvoiceDocument invoiceDocument,
            IDialogService dialogService, IConfigurationService configurationService)
        {
            _invoiceDocument = invoiceDocument;
            _dialogService = dialogService;
            _configurationService = configurationService;
        }
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
                ShowErrorDialog("Warnung", "Wählen Sie einen Kunden bevor Sie eine Rechnung schreiben.",
                    "Ok",
                    "Warning",
                    Visibility.Collapsed, "", null);     
                return;
            }
            if (createInvoiceModel.InvoiceDate is null)
            {
                ShowErrorDialog("Warnung", "Das Rechnungsdatum fehlt.",
                      "Ok",
                      "Warning",
                      Visibility.Collapsed, "", null);
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
            invoiceModel.IssueDate = createInvoiceModel.InvoiceDate;
            invoiceModel.DueDate = DateTime.Now;
            invoiceModel.NetAmount = createInvoiceModel?.Positions.Sum(x => x.Price * x.Quantity);
            invoiceModel.State = InvoiceState.Open;
            invoiceModel.IsMwstApplicable = createInvoiceModel.IsMwstApplicable;
            invoiceModel.CommentsTop = createInvoiceModel.CommentsTop;
            invoiceModel.CommentsBottom = createInvoiceModel.CommentsBottom;
            invoiceModel.Logo = createInvoiceModel.Logo;
            if (createInvoiceModel.StartDate.HasValue)
                invoiceModel.StartDate = createInvoiceModel.StartDate;
            if (createInvoiceModel.EndDate.HasValue)
                invoiceModel.EndDate = createInvoiceModel.EndDate;

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
            }

            await CreateInvoice(createInvoiceModel);
        }

        private async Task CreateInvoice(CreateInvoiceModel createInvoiceModel)
        {
            IEnumerable<InvoicePosition> positions = createInvoiceModel.Positions;
            UserSettings userSettings = _configurationService.GetUserSettings();
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(createInvoiceModel.Client, positions, _invoiceModel, userSettings);
            model.IsMwstApplicable = createInvoiceModel.IsMwstApplicable;
            if (createInvoiceModel.StartDate.HasValue)
                model.StartDate = createInvoiceModel.StartDate;
            if (createInvoiceModel.EndDate.HasValue)
                model.EndDate = createInvoiceModel.EndDate;
            model.CommentsTop = createInvoiceModel.CommentsTop;
            model.CommentsBottom = createInvoiceModel.CommentsBottom;
            model.Logo = createInvoiceModel.Logo;
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
                UnitOfMeasure = position.UnitOfMeasure != null ? position.UnitOfMeasure.Name : "stk",
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
            Visibility okButtonVisibility, string okButtonText, CreateInvoiceModel? createInvoiceModel)
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