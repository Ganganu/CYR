using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Invoice.InvoiceServices;
using CYR.Services;
using System.Collections.ObjectModel;

namespace CYR.Invoice.InvoiceViewModels
{
    public partial class ShowInvoiceViewModel : ObservableObject, IParameterReceiver
    {
        private readonly IInvoicePositionRepository _invoicePositionRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPreviewInvoiceService _previewInvoiceService;

        public ShowInvoiceViewModel(INavigationService navigationService, IInvoicePositionRepository invoicePositionRepository,
            IInvoiceRepository invoiceRepository,
            IPreviewInvoiceService previewInvoiceService)
        {
            NavigationService = navigationService;
            _invoicePositionRepository = invoicePositionRepository;
            _invoiceRepository = invoiceRepository;
            _previewInvoiceService = previewInvoiceService;
        }
        public INavigationService NavigationService { get; }

        [ObservableProperty]
        private InvoiceModel? _invoiceModel;
        [ObservableProperty]
        private ObservableCollection<InvoicePositionModel>? _items;

        [RelayCommand]
        private void NavigateBack()
        {
            NavigationService.NavigateTo<InvoiceListViewModel>();
        }

        public async Task ReceiveParameter(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            InvoiceModel = await _invoiceRepository.GetByIdAsync((int)parameter);
            IEnumerable<InvoicePositionModel> items = await _invoicePositionRepository.GetAllPositionsByInvoiceIdAsync(InvoiceModel.InvoiceNumber);
            List<InvoicePositionModel> listItems = [.. items];
            for (int i = 0; i < listItems.Count; i++)
            {
                listItems[i].Id = (i + 1).ToString();
            }
            Items = [.. listItems];
        }
        [RelayCommand]
        private async Task PreviewInvoice()
        {
            if (InvoiceModel is null)
            {
                return;
            }
            CreateInvoiceModel createInvoiceModel = new()
            {
                Client = new Client
                {
                    ClientNumber = InvoiceModel.Customer.ClientNumber,
                    City = InvoiceModel.Customer.City,
                    EmailAddress = InvoiceModel.Customer.EmailAddress,
                    Name = InvoiceModel.Customer.Name,
                    PLZ = InvoiceModel.Customer.PLZ,
                    Street = InvoiceModel.Customer.Street
                },
                EndDate = DateTime.Parse(InvoiceModel.EndDate),
                InvoiceDate = DateTime.Parse(InvoiceModel.IssueDate),
                InvoiceNumber = InvoiceModel.InvoiceNumber,
                IsMwstApplicable = InvoiceModel.Mwst,
                ObjectNumber = InvoiceModel.ObjectNumber,
                Positions = ConvertInvoicePositionModelToInvoicePosition(Items),
                //Notiz = Notiz,
                StartDate = DateTime.Parse(InvoiceModel.StartDate),
                Subject = InvoiceModel.Subject
            };
            await _previewInvoiceService.SaveInvoice(createInvoiceModel);
        }

        private ObservableCollection<InvoicePosition> ConvertInvoicePositionModelToInvoicePosition(ObservableCollection<InvoicePositionModel>? items)
        {
            ObservableCollection<InvoicePosition> invoicePositions = [];
            if (items is null) return [];
            foreach (var item in items)
            {
                InvoicePosition invoicePosition = new()
                {
                    OrderItem = new OrderItem.OrderItem
                    {
                        Id = Convert.ToInt32(item.Id),
                        Description = item.Description,
                        Name = item.Description,
                        Price = item.UnitPrice
                    },
                    UnitOfMeasure = new()
                    {
                        Name = item.UnitOfMeasure
                    },
                    InvoiceNumber = item.InvoiceNumber,
                    Price = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                };
                invoicePositions.Add(invoicePosition);
            }
            return invoicePositions;
        }
    }
}
