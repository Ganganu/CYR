using CYR.Clients;
using CYR.Dialog;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceViewModels;
using CYR.Messages;
using CYR.OrderItems;
using CYR.Services;
using CYR.Settings;
using QuestPDF.Fluent;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.Invoice.InvoiceServices;

public class PreviewInvoiceService : IPreviewInvoiceService
{
    private readonly IInvoiceDocument _invoiceDocument;
    private readonly IConfigurationService _configurationService;
    private InvoiceModel? _invoiceModel;
    private string? _dialogResponse;
    public PreviewInvoiceService(IInvoiceDocument invoiceDocument,IConfigurationService configurationService)
    {
        _invoiceDocument = invoiceDocument;
        _configurationService = configurationService;
    }
    public async Task<SnackbarMessage> PreviewInvoice(CreateInvoiceModel createInvoiceModel)
    {
        if (createInvoiceModel.InvoiceNumber is null) return new SnackbarMessage("Rechnungsnummer fehlt!", "Error");
        if (createInvoiceModel.InvoiceDate is null) return new SnackbarMessage("Rechnungsdatum fehlt!", "Error");
        if (createInvoiceModel.Client is null) return new SnackbarMessage("Wählen Sie bitte einen Kunden aus.", "Error");
        if (createInvoiceModel.Positions is null) return new SnackbarMessage("Fehler aufgetreten!", "Error");
        if (createInvoiceModel.Positions.Count <= 0) return new SnackbarMessage("Keine Positionen in Rechnung.", "Error");
        if (createInvoiceModel.Positions.Any(p => p.Price < 0)) return new SnackbarMessage("Der Preis eines ausgewählten Artikels ist kleiner 0.", "Error");

        var invalidPositions = createInvoiceModel.Positions
        .Select((p, index) => new { Position = p, Index = index + 1 })
        .Where(p =>
            p.Position.OrderItem == null ||
            !decimal.TryParse(p.Position.Quantity?.ToString(), out decimal qty) || qty < 0 ||
            string.IsNullOrWhiteSpace(p.Position.OrderItem?.Name)
        )
        .ToList();
        if (invalidPositions.Count != 0)
        {
            string problemDetails = string.Join(", ", invalidPositions.Select(p => $"#{p.Index}"));
            return new SnackbarMessage($"Die Position(en) {problemDetails} enthalten Fehler!", "Error");
        }

        Client client = new Client
        {
            ClientNumber = createInvoiceModel.Client.ClientNumber
        };            

        InvoiceModel invoiceModel = new InvoiceModel();
        invoiceModel.InvoiceNumber = createInvoiceModel.InvoiceNumber;
        invoiceModel.Customer = client;
        invoiceModel.IssueDate = createInvoiceModel.InvoiceDate;
        invoiceModel.DueDate = DateTime.Now;
        invoiceModel.NetAmount = createInvoiceModel?.Positions.Sum(x => x.Price * Convert.ToDecimal(x.Quantity));
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
        }

        await CreateInvoice(createInvoiceModel);
        return new SnackbarMessage("Preview erfolgreich erstellt.", "Check");
    }

    private async Task CreateInvoice(CreateInvoiceModel createInvoiceModel)
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

    private static InvoicePositionModel CreateInvoicePositionModel(OrderItem orderItem, InvoicePosition position, InvoiceModel invoiceModel)
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

    private static OrderItem ManuallyInsertedItemToOrderItem(InvoicePosition invoicePosition)
    {
        OrderItem item = new OrderItem();
        item.Name = invoicePosition.ManuallyInsertedArticle;
        item.Price = invoicePosition.Price;
        item.Description = invoicePosition.ManuallyInsertedArticle;
        return item;
    }    
}