using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Clients;
using CYR.Core;
using CYR.Dialog;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Invoice.InvoiceServices;
using CYR.Logging;
using CYR.Messages;
using CYR.OrderItems;
using CYR.Services;
using CYR.Settings;
using CYR.UnitOfMeasure;
using CYR.User;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows.Media;

namespace CYR.Invoice.InvoiceViewModels;

public partial class CreateInvoiceViewModel : ObservableRecipientWithValidation, IRecipient<InvoiceTotalPriceEvent>,
                                            IRecipient<InvoiceMwstEvent>, IRecipient<ItemsListDialogViewModel>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
    private readonly ISaveInvoiceInvoicePositionService _saveInvoiceInvoicePositionService;
    private readonly IPreviewInvoiceService _previewInvoiceService;
    private readonly IRetrieveClients _retrieveClients;
    private readonly IOpenImageService _openImageService;
    private readonly ISelectImageService _selectImageService;
    private readonly IFileService _fileService;
    private readonly IDialogService _dialogService;
    private readonly LoggingRepository _loggingRepository;
    private readonly UserContext _userContext;

    private string? _dialogResponse;
    private readonly string _directoryPath = AppDomain.CurrentDomain.BaseDirectory;

    private int _positionCounter = 1;
    private Client? _client;

    public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository,
        IUnitOfMeasureRepository unitOfMeasureRepository,
        INavigationService navigationService,
        ISaveInvoiceInvoicePositionService saveInvoiceInvoicePositionService,
        IPreviewInvoiceService previewInvoiceService,
        IRetrieveClients retrieveClients,
        IOpenImageService openImageService,
        ISelectImageService selectImageService,
        IDialogService dialogService,
        IFileService fileService,
        LoggingRepository loggingRepository,
        UserContext userContext)
    {
        _orderItemRepository = orderItemRepository;
        _unitOfMeasureRepository = unitOfMeasureRepository;
        NavigationService = navigationService;
        _saveInvoiceInvoicePositionService = saveInvoiceInvoicePositionService;
        _previewInvoiceService = previewInvoiceService;
        _retrieveClients = retrieveClients;
        _openImageService = openImageService;
        InvoiceModel = new InvoiceModel();
        Initialize();
        Messenger.RegisterAll(this);
        _selectImageService = selectImageService;
        _dialogService = dialogService;
        _fileService = fileService;
        _loggingRepository = loggingRepository;
        _userContext = userContext;
    }
    private async void Initialize()
    {
        Positions = new ObservableCollection<InvoicePosition> { new(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString() } };
        IEnumerable<Client> cl = await _retrieveClients.Handle();
        Clients = [.. cl];
    }

    [ObservableProperty]
    private InvoiceModel _invoiceModel;
    [ObservableProperty]
    private ObservableCollection<Client> _clients;
    [ObservableProperty]
    private Client? _selectedClient;
    [ObservableProperty]
    private ImageSource _logo;
    [ObservableProperty]
    private decimal? _totalPrice = 0.0m;
    [ObservableProperty]
    private ObservableCollection<FileModel> _xmlFiles;
    public INavigationService NavigationService { get; }

    partial void OnSelectedClientChanged(Client? oldValue, Client? newValue)
    {
        if (newValue != oldValue)
        {
            _client = newValue;
        }
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
    private async Task SaveArticle(object parameters)
    {        
        var selectedPositions = Positions?.Where(p => p.IsInvoicePositionSelected).ToList();
        if (selectedPositions is null) return;
        if (selectedPositions.Any(p => p.HasErrors))
        {
            Messenger.Send(new SnackbarMessage($"Die eingaben enthalten Fehler!", "Error"));
            return;
        }
        foreach (var position in selectedPositions)
        {
            OrderItem orderItem = new();
            orderItem.Name = position.ManuallyInsertedArticle;
            orderItem.Description = position.ManuallyInsertedArticle;
            orderItem.Price = position.Price;
            await _orderItemRepository.InsertAsync(orderItem);
            await _loggingRepository.InsertAsync(CreateHisModel(position));
        }
    }

    [RelayCommand]
    private void NavigateBack()
    {
        NavigationService.NavigateTo<InvoiceListViewModel>();
    }

    [RelayCommand]
    private async Task PreviewInvoice()
    {
        CreateInvoiceModel createInvoiceModel = new()
        {
            Client = _client,
            InvoiceDate = InvoiceModel.IssueDate,
            InvoiceNumber = InvoiceModel.InvoiceNumber,
            IsMwstApplicable = InvoiceModel.IsMwstApplicable,
            Positions = Positions,
            CommentsBottom = InvoiceModel.CommentsBottom,
            CommentsTop = InvoiceModel.CommentsTop
        };
        var message = await _previewInvoiceService.PreviewInvoice(createInvoiceModel);
        Messenger.Send(message);
    }
    [RelayCommand]
    private async Task SaveInvoice()
    {
        if (Positions.Any(p => p.HasErrors))
        {
            Messenger.Send(new SnackbarMessage($"Die eingaben enthalten Fehler!", "Error"));
            return;
        }

        CreateInvoiceModel createInvoiceModel = new()
        {
            Client = _client,
            InvoiceDate = InvoiceModel.IssueDate,
            InvoiceNumber = InvoiceModel.InvoiceNumber,
            IsMwstApplicable = InvoiceModel.IsMwstApplicable,
            Positions = Positions,
            CommentsBottom = InvoiceModel.CommentsBottom,
            CommentsTop = InvoiceModel.CommentsTop

        };
        var message = await _saveInvoiceInvoicePositionService.SaveInvoice(createInvoiceModel);
        Messenger.Send(message);
        await _loggingRepository.InsertAsync(CreateHisModel(createInvoiceModel));
    }


    [RelayCommand]
    private void DeleteInvoicePosition()
    {
        var selectedPositions = Positions?.Where(p => p.IsInvoicePositionSelected).ToList();
        if (selectedPositions is null) return;
        foreach (var position in selectedPositions)
        {
            Positions?.Remove(position);
            _positionCounter--;
        }
        int posCounter = 1;
        foreach (var position in Positions)
        {
            position.Id = posCounter.ToString();
            posCounter++;
        }
        TotalPrice = Positions?.Sum(p => p.TotalPrice);
    }
    [RelayCommand]
    private void OpenImageInDefaultApp()
    {
        string imagePath = new Uri(Logo.ToString()).LocalPath;
        _openImageService.OpenImage(imagePath);
    }
    [RelayCommand]
    private void SelectLogo()
    {
        var message = _selectImageService.SelectImage();
        Logo = message.ImageSource;
        Messenger.Send(message);    
    }

    public void Receive(InvoiceTotalPriceEvent message)
    {
        TotalPrice = 0;
        foreach (var item in Positions)
        {
            TotalPrice += item.TotalPrice;
        }
    }

    public void Receive(InvoiceMwstEvent message)
    {
        if (message.isMwstApplicable == true) TotalPrice *= 1.19m;
        else TotalPrice /= 1.19m;
    }
    [RelayCommand]
    private void SaveXmlTop(object parameter)
    {
        var dataContext = (CreateInvoiceViewModel)parameter;
        var xml = dataContext.InvoiceModel.CommentsTop;
        if (xml is null) return;
        ShowCommentsDialog("Vorlage speichern", "FileDocumentPlus", xml);
    }
    [RelayCommand]
    private void SaveXmlBottom(object parameter)
    {
        var dataContext = (CreateInvoiceViewModel)parameter;
        var xml = dataContext.InvoiceModel.CommentsBottom;
        if (xml is null) return;
        ShowCommentsDialog("Vorlage speichern", "FileDocumentPlus", xml);
    }
    [RelayCommand]
    private void LoadXmlTop()
    {
        string commentsPath = $@"{_directoryPath}\Comments\Top";
        string folderPath = @"\Comments\Top";
        List<FileModel> files = _fileService.LoadFileNamesFromPath(commentsPath);
        XmlFiles = [.. files];
        ShowListDialog("Rechnungskopftext Vorlagen", XmlFiles, "File", folderPath, CommentType.Top);
    }
    [RelayCommand]
    private void LoadXmlBottom()
    {
        string commentsPath = $@"{_directoryPath}\Comments\Bottom";
        string folderPath = @"\Comments\Bottom";
        List<FileModel> files = _fileService.LoadFileNamesFromPath(commentsPath);
        XmlFiles = [.. files];
        ShowListDialog("Rechnungsfußtext Vorlagen", XmlFiles, "File", folderPath, CommentType.Bottom);
    }

    
    private void ShowListDialog(string title, ObservableCollection<FileModel> files, string icon, string folderPath, CommentType type)
    {
        _dialogService.ShowDialog(result =>
        {
            _dialogResponse = result;
        },
        new Dictionary<Expression<Func<ItemsListDialogViewModel, object>>, object>
        {
            { vm => vm.Title, title },
            { vm => vm.Files,  files},
            { vm => vm.Icon,  icon},
            {vm => vm.FolderPath, folderPath},
            {vm => vm.CommentType, type},
        });
    }
    private void ShowCommentsDialog(string title, string icon, string text)
    {
        _dialogService.ShowDialog(result =>
        {
            _dialogResponse = result;
        },
        new Dictionary<Expression<Func<SaveCommentsDialogViewModel, object>>, object>
        {
            { vm => vm.Title, title },
            { vm => vm.Icon,  icon},
            {vm => vm.TextToSerialize, text }
        });
    }

    public void Receive(ItemsListDialogViewModel message)
    {
        switch (message.CommentType)
        {
            case CommentType.Top:
                InvoiceModel.CommentsTop = message.XmlString;
                break;
            case CommentType.Bottom:
                InvoiceModel.CommentsBottom = message.XmlString;
                break;
            default:
                break;
        }
    }


    private HisModel CreateHisModel(CreateInvoiceModel createInvoiceModel)
    {
        HisModel hisModel = new();
        hisModel.LoggingType = LoggingType.InvoiceCreated;
        hisModel.InvoiceId = createInvoiceModel.InvoiceNumber;
        hisModel.UserId = _userContext.CurrentUser.Id;
        hisModel.Message = $@"Rechnung {createInvoiceModel.InvoiceNumber} erfolgreich erstellt.";
        return hisModel;
    }

    private HisModel CreateHisModel(InvoicePosition position)
    {
        HisModel hisModel = new HisModel();
        hisModel.LoggingType = LoggingType.OrderItemCreated;
        hisModel.OrderItemId = position.Id;
        hisModel.UserId = _userContext.CurrentUser.Id;
        hisModel.Message = $@"Artikel {hisModel.OrderItemId} erfolgreich erstellt.";
        return hisModel;
    }
}