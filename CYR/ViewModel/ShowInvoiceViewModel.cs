using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Invoice;
using CYR.Services;

namespace CYR.ViewModel
{
    public partial class ShowInvoiceViewModel : ObservableObject, IParameterReceiver
    {
        public ShowInvoiceViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
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


        [RelayCommand]
        private void NavigateBack()
        {
            NavigationService.NavigateTo<GetInvoiceViewModel>();
        }

        public void ReceiveParameter(object parameter)
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
        }
    }
}
