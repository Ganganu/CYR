using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Clients;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Invoice.InvoiceServices;
using CYR.OrderItems;
using CYR.PDF;
using CYR.Services;
using CYR.Settings;
using CYR.UnitOfMeasure;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Windows.Media;

namespace CYR.Invoice.InvoiceViewModels
{
    public partial class CreateInvoiceViewModel : ObservableRecipient, IRecipient<LogoEvent>, IRecipient<InvoiceTotalPriceEvent>, IParameterReceiver,
                                                IRecipient<InvoiceMwstEvent>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;
        private readonly ISaveInvoiceInvoicePositionService _saveInvoiceInvoicePositionService;
        private readonly IPreviewInvoiceService _previewInvoiceService;
        private readonly IRetrieveClients _retrieveClients;
        private readonly IConfigurationService _configurationService;
        private readonly IOpenImageService _openImageService;
        private readonly UserSettings _userSettings;
        private readonly ISelectImageService _selectImageService;
        private readonly IXMLService _xmlService;

        private int _positionCounter = 1;
        private Client? _client;

        public CreateInvoiceViewModel(IOrderItemRepository orderItemRepository,
            IUnitOfMeasureRepository unitOfMeasureRepository,
            INavigationService navigationService,
            ISaveInvoiceInvoicePositionService saveInvoiceInvoicePositionService,
            IPreviewInvoiceService previewInvoiceService,
            IRetrieveClients retrieveClients,
            IConfigurationService configurationService,
            IOpenImageService openImageService,
            ISelectImageService selectImageService,
            IXMLService xmlService)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfMeasureRepository = unitOfMeasureRepository;
            NavigationService = navigationService;
            _saveInvoiceInvoicePositionService = saveInvoiceInvoicePositionService;
            _previewInvoiceService = previewInvoiceService;
            _retrieveClients = retrieveClients;
            _configurationService = configurationService;
            _openImageService = openImageService;
            _userSettings = _configurationService.GetUserSettings();
            InvoiceModel = new InvoiceModel();
            Initialize();
            Messenger.RegisterAll(this);
            _selectImageService = selectImageService;
            _xmlService = xmlService;
        }
        private async void Initialize()
        {
            Positions = new ObservableCollection<InvoicePosition> { new(_orderItemRepository, _unitOfMeasureRepository) { Id = _positionCounter.ToString() } };
            IEnumerable<Client> cl = await _retrieveClients.Handle();
            Clients = [.. cl];
            Logo = _userSettings.Logo;
            InvoiceModel.Logo = Logo;
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
            foreach (var position in selectedPositions)
            {
                OrderItem.OrderItem orderItem = new();
                orderItem.Name = position.ManuallyInsertedArticle;
                orderItem.Description = position.ManuallyInsertedArticle;
                orderItem.Price = position.Price;
                await _orderItemRepository.InsertAsync(orderItem);
            }
        }

        [RelayCommand]
        private async Task PreviewInvoice()
        {
            CreateInvoiceModel createInvoiceModel = new()
            {
                Client = _client,
                EndDate = InvoiceModel.EndDate,
                InvoiceDate = InvoiceModel.IssueDate,
                InvoiceNumber = InvoiceModel.InvoiceNumber,
                IsMwstApplicable = InvoiceModel.IsMwstApplicable,
                Positions = Positions,
                StartDate = InvoiceModel.StartDate,
                CommentsBottom = InvoiceModel.CommentsBottom,
                CommentsTop = InvoiceModel.CommentsTop,
                Logo = InvoiceModel.Logo
            };
            await _previewInvoiceService.SaveInvoice(createInvoiceModel);
        }
        [RelayCommand]
        private async Task SaveInvoice()
        {
            CreateInvoiceModel createInvoiceModel = new()
            {
                Client = _client,
                EndDate = InvoiceModel.EndDate,
                InvoiceDate = InvoiceModel.IssueDate,
                InvoiceNumber = InvoiceModel.InvoiceNumber,
                IsMwstApplicable = InvoiceModel.IsMwstApplicable,
                Positions = Positions,
                StartDate = InvoiceModel.StartDate,
                CommentsBottom = InvoiceModel.CommentsBottom,
                CommentsTop = InvoiceModel.CommentsTop,
                Logo = InvoiceModel.Logo

            };
            await _saveInvoiceInvoicePositionService.SaveInvoice(createInvoiceModel);
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
            Logo = _selectImageService.SelectImage();
        }

        public void Receive(LogoEvent message)
        {
            Logo = message.Logo;
        }

        public void Receive(InvoiceTotalPriceEvent message)
        {
            TotalPrice = 0;
            foreach (var item in Positions)
            {
                TotalPrice += item.TotalPrice;
            }
        }

        public async Task ReceiveParameter(object parameter)
        {
            if (parameter is null) return;
            CreateInvoiceModel createInvoiceModel = (CreateInvoiceModel)parameter;
            Positions = createInvoiceModel.Positions;
        }

        public void Receive(InvoiceMwstEvent message)
        {
            if (message.isMwstApplicable == true) TotalPrice *= 1.19m;
            else TotalPrice /= 1.19m;
        }
        partial void OnLogoChanged(ImageSource? oldValue, ImageSource newValue)
        {
            if (oldValue != newValue)
            {
                InvoiceModel.Logo = Logo;
            }
        }
        [RelayCommand]
        private void SaveXml(object parameter)
        {
            var dataContext = (CreateInvoiceViewModel)parameter;
            var xml = dataContext.InvoiceModel.CommentsTop;
            _xmlService.SaveAsync(xml);
        }
        [RelayCommand]
        private void LoadXml()
        {
            RunParser runParser = new();
            string comments = _xmlService.LoadAsync();
            var runs = runParser.GetRunsAndData(comments);
            foreach (var run in runs)
            {
                var fontSize = run.FontSize;
                var fontWeight = StringToQuestPdfConverter.ToFontWeight(run.FontWeight);
                var fontStyle = run.FontStyle;
                var textDecorations = run.TextDecorations;
                var text = run.Text;
                string xamlText = "<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                     "<Paragraph><Run FontWeight=\"Bold\">" + System.Security.SecurityElement.Escape(text) + "</Run></Paragraph>" +
                     "</FlowDocument>";
                InvoiceModel.CommentsTop = xamlText;
                //InvoiceModel.CommentsTop = run.Text;
            }
        }
    }
}