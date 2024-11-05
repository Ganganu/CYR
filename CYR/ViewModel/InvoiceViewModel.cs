using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Services;

namespace CYR.ViewModel
{
    public partial class InvoiceViewModel : ObservableObject
    {
        public InvoiceViewModel(INavigationService navigationService)
        {
            Navigation = navigationService;
        }

        [ObservableProperty]
        private INavigationService _navigation;

        [RelayCommand]
        private void NavigateToCreateNewInvoice()
        {
            Navigation.NavigateTo<CreateInvoiceViewModel>();
        }
        [RelayCommand]
        private void NavigateToGetInvoices()
        {
            Navigation.NavigateTo<GetInvoiceViewModel>();
        }
    }
}
