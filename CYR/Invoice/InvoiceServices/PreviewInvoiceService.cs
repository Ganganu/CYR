using CYR.Clients;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceViewModels;
using CYR.Messages;
using CYR.OrderItems;
using CYR.Services;
using CYR.User;
using QuestPDF.Fluent;

namespace CYR.Invoice.InvoiceServices;

public class PreviewInvoiceService : IPreviewInvoiceService
{
    private readonly IInvoiceDocument _invoiceDocument;
    private InvoiceModel? _invoiceModel;
    private readonly UserCompanyRepository _userCompanyRepository;
    private readonly UserContext _userContext;
    private string? _dialogResponse;
    public PreviewInvoiceService(IInvoiceDocument invoiceDocument, UserCompanyRepository userCompanyRepository, UserContext userContext)
    {
        _invoiceDocument = invoiceDocument;
        _userCompanyRepository = userCompanyRepository;
        _userContext = userContext;
    }
    public async Task<SnackbarMessage> PreviewInvoice(CreateInvoiceModel createInvoiceModel)
    {
        if (createInvoiceModel.InvoiceNumber is null) return new SnackbarMessage("Rechnungsnummer fehlt!", "Error");
        if (createInvoiceModel.InvoiceDate is null) return new SnackbarMessage("Rechnungsdatum fehlt!", "Error");
        if (createInvoiceModel.Client is null) return new SnackbarMessage("Wählen Sie bitte einen Kunden aus.", "Error");
        if (createInvoiceModel.Positions is null) return new SnackbarMessage("Fehler aufgetreten!", "Error");
        if (createInvoiceModel.Positions.Count <= 0) return new SnackbarMessage("Keine Positionen in Rechnung.", "Error");
        if (createInvoiceModel.Positions.Any(p => Convert.ToDecimal(p.Price) < 0)) return new SnackbarMessage("Der Preis eines ausgewählten Artikels ist kleiner 0.", "Error");

        var invalidPositions = createInvoiceModel.Positions
        .Select((p, index) => new { Position = p, Index = index + 1 })
        .Where(p =>
            p.Position.OrderItem == null ||
            !decimal.TryParse(p.Position.Quantity?.ToString(), out decimal qty) || qty < 0 ||
            (string.IsNullOrWhiteSpace(p.Position.OrderItem?.Name) && string.IsNullOrWhiteSpace(p.Position.OrderItem?.Description))
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
        invoiceModel.NetAmount = createInvoiceModel?.Positions.Sum(x => Convert.ToDecimal(x.Price) * Convert.ToDecimal(x.Quantity));
        invoiceModel.State = InvoiceState.Open;
        invoiceModel.IsMwstApplicable = createInvoiceModel.IsMwstApplicable;
        invoiceModel.CommentsTop = createInvoiceModel.CommentsTop;
        invoiceModel.CommentsBottom = createInvoiceModel.CommentsBottom;           

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

        var message = await CreateInvoice(createInvoiceModel);
        return message;
    }

    private async Task<SnackbarMessage> CreateInvoice(CreateInvoiceModel createInvoiceModel)
    {
        IEnumerable<InvoicePosition> positions = createInvoiceModel.Positions;
        int id = Convert.ToInt32(_userContext.CurrentUser.Id);
        UserCompany userSettings = await _userCompanyRepository.GetAsync(id);
        if (userSettings is null) return new SnackbarMessage("Ihre Firmendaten sind unvollständig. Bitte vervollständigen Sie diese, bevor Sie eine Rechnung erstellen.", "Error");
        var model = InvoiceDocumentDataSource.GetInvoiceDetails(createInvoiceModel.Client, positions, _invoiceModel, userSettings);
        model.IsMwstApplicable = createInvoiceModel.IsMwstApplicable;            
        model.CommentsTop = createInvoiceModel.CommentsTop;
        model.CommentsBottom = createInvoiceModel.CommentsBottom;
        _invoiceDocument.Model = model;
        try
        {
            _invoiceDocument.GeneratePdfAndShow();
            return new SnackbarMessage($"Die Rechnung mit der Rechnungsnummer {createInvoiceModel.InvoiceNumber} wurde erfolgreich gespeichert!", "Check");
        }
        catch (Exception)
        {
            return new SnackbarMessage("Beim Erstellen der Rechnung ist ein Problem aufgetreten.", "Error");
        }
    }

    private static InvoicePositionModel CreateInvoicePositionModel(OrderItem orderItem, InvoicePosition position, InvoiceModel invoiceModel)
    {
        var invoicePositionModel = new InvoicePositionModel
        {
            InvoiceNumber = invoiceModel.InvoiceNumber.ToString(),
            Description = orderItem.Description,
            Quantity = position.Quantity,
            UnitOfMeasure = position.UnitOfMeasure != null ? position.UnitOfMeasure.Name : "stk",
            UnitPrice = Convert.ToDecimal(orderItem.Price),
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