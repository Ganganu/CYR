using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Invoice;
using CYR.Services;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class ShowInvoiceViewModel : ObservableObject, IParameterReceiver
    {        
        private readonly IInvoicePositionRepository _invoicePositionRepository;

        public ShowInvoiceViewModel(INavigationService navigationService, IInvoicePositionRepository invoicePositionRepository)
        {
            NavigationService = navigationService;
            _invoicePositionRepository = invoicePositionRepository;            
        }
        public INavigationService NavigationService { get; }

        [ObservableProperty]
        private string? _clientName;
        [ObservableProperty]
        private string? _clientZip;
        [ObservableProperty]
        private string? _clientCity;
        [ObservableProperty]
        private string? _clientStreet;
        [ObservableProperty]
        private string? _subject;
        [ObservableProperty]
        private string? _objectNumber;
        [ObservableProperty]
        private string? _startDate;
        [ObservableProperty]
        private string? _endDate;
        [ObservableProperty]
        private ObservableCollection<InvoicePositionModel> _items;

        [RelayCommand]
        private void NavigateBack()
        {
            NavigationService.NavigateTo<GetInvoiceViewModel>();
        }

        public async void ReceiveParameter(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            InvoiceModel model = (InvoiceModel)parameter;
            ClientName = model.Customer.Name;
            ClientCity = model.Customer.City;
            ClientStreet = model.Customer.Street;
            ClientZip = model.Customer.PLZ;
            Subject = model.Subject;
            ObjectNumber = model.ObjectNumber;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            IEnumerable<InvoicePositionModel> items = await _invoicePositionRepository.GetAllPositionsByInvoiceIdAsync(model.InvoiceNumber);
            Items = new ObservableCollection<InvoicePositionModel>(items);
        }
    }
}
