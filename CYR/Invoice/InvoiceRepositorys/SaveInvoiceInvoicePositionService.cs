using CYR.Clients;
using System.Data.SQLite;
using System.Windows;
using CYR.Core;
using CYR.ViewModel;
using CYR.Dialog;
using System.Linq.Expressions;
using CYR.Services;
using CYR.Settings;
using QuestPDF.Fluent;
using CYR.Invoice.InvoiceViewModels;
using CYR.Invoice.InvoiceModels;
using CYR.OrderItems;

namespace CYR.Invoice.InvoiceRepositorys;

public class SaveInvoiceInvoicePositionService : ISaveInvoiceInvoicePositionService
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoicePositionRepository _invoicePositionRepository;
    private readonly IDialogService _dialogService;
    private readonly IConfigurationService _configurationService;
    private readonly IInvoiceDocument _invoiceDocument;
    private InvoiceModel _invoiceModel;
    private string? _dialogResponse;
    public SaveInvoiceInvoicePositionService(IDatabaseConnection databaseConnection, IInvoiceRepository invoiceRepository,
        IInvoicePositionRepository invoicePositionRepository, IDialogService dialogService, IConfigurationService configurationService, 
        IInvoiceDocument invoiceDocument)
    {
        _databaseConnection = databaseConnection;
        _invoiceRepository = invoiceRepository;
        _invoicePositionRepository = invoicePositionRepository;
        _dialogService = dialogService;
        _configurationService = configurationService;
        _invoiceDocument = invoiceDocument;
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
        bool checkPositionNull = createInvoiceModel.Positions.Any(p => p.OrderItem == null || p.Quantity <= 0 || p.OrderItem.Name == null);
        if (checkPositionNull)
        {
            ShowErrorDialog("Fehler", "Die ausgewählten Artikel enthalten Problemen!",
                            "Abbrechen",
                            "Warning",
                            Visibility.Collapsed, "", createInvoiceModel);
            return;
        }
        using (var connection = new SQLiteConnection(_databaseConnection.ConnectionString))
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    if (createInvoiceModel.Client == null)
                    {
                        return;
                    }
                    // Create and populate invoice
                    Client client = new Client
                    {
                        ClientNumber = createInvoiceModel.Client.ClientNumber
                    };

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
                    if (createInvoiceModel.IsMwstApplicable)
                    {
                        invoiceModel.GrossAmount = Math.Round((decimal)invoiceModel.NetAmount * 1.19m, 2);
                    }
                    else
                    {
                        invoiceModel.GrossAmount = invoiceModel.NetAmount;
                    }
                    _invoiceModel = invoiceModel;

                    // Save invoice
                    await _invoiceRepository.InsertAsync(invoiceModel, transaction);

                    // Save each position
                    foreach (var position in createInvoiceModel.Positions)
                    {
                        InvoicePositionModel ipm;
                        if (position.OrderItem is not null && position.OrderItem.Id == 0)
                        {
                            OrderItem manuallyInsertedOrderItem = ManuallyInsertedItemToOrderItem(position);
                            ipm = CreateInvoicePositionModel(manuallyInsertedOrderItem, position, invoiceModel);
                            position.OrderItem.Name = manuallyInsertedOrderItem.Name;
                            position.OrderItem.Description = manuallyInsertedOrderItem.Description;
                        }
                        else
                        {
                            ipm = CreateInvoicePositionModel(position.OrderItem, position, invoiceModel);
                        }
                        await _invoicePositionRepository.InsertAsync(ipm, transaction);
                    }

                    transaction.Commit();
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();
                    ShowErrorDialog("Fehler", $"Die Rechnung mit der Rechnungsnummer {createInvoiceModel.InvoiceNumber} existiert bereits!" +
                    $"Ändern Sie die Rechnungsnummer und versuchen Sie es erneut.", "Abbrechen", "Error", Visibility.Collapsed,
                    "", createInvoiceModel);
                    return;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            CreateInvoice(createInvoiceModel);
        }
    }

    private void CreateInvoice(CreateInvoiceModel createInvoiceModel)
    {
        IEnumerable<InvoicePosition> positions = createInvoiceModel.Positions;
        UserSettings userSettings = _configurationService.GetUserSettings();
        var model = InvoiceDocumentDataSource.GetInvoiceDetails(createInvoiceModel.Client, positions, _invoiceModel, userSettings);
        model.IsMwstApplicable = createInvoiceModel.IsMwstApplicable;            
        model.CommentsTop = createInvoiceModel.CommentsTop;
        model.CommentsBottom = createInvoiceModel.CommentsBottom;
        model.Logo = createInvoiceModel.Logo;
        _invoiceDocument.Model = model;
        _invoiceDocument.GeneratePdfAndShow();
    }

    private InvoicePositionModel CreateInvoicePositionModel(OrderItem orderItem, InvoicePosition position, InvoiceModel invoiceModel)
    {
        var invoicePositionModel = new InvoicePositionModel
        {
            InvoiceNumber = invoiceModel.InvoiceNumber.ToString(),
            Description = orderItem.Description,
            Quantity = position.Quantity,
            UnitOfMeasure = position.UnitOfMeasure != null ? position.UnitOfMeasure.Name : "",
            UnitPrice = orderItem.Price,
            TotalPrice = position.TotalPrice
        };
        return invoicePositionModel;
    }

    private OrderItem ManuallyInsertedItemToOrderItem(InvoicePosition invoicePosition)
    {
        OrderItem item = new OrderItem();
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