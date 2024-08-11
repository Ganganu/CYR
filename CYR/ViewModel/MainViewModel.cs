using CommunityToolkit.Mvvm.Input;
using CYR.Core;
using CYR.Services;

namespace CYR.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationService navigationService) 
        {
            Navigation = navigationService;
        }

        private INavigationService _navigation;

        public INavigationService Navigation
        {
            get { return _navigation; }
            set 
            { 
                _navigation = value; 
                OnPropertyChanged(nameof(Navigation));
            }
        }
        [RelayCommand]
        private void NavigateToClientView()
        {
            Navigation.NavigateTo<ClientViewModel>();
        }
        [RelayCommand]
        private void NavigateToInvoiceView()
        {
            Navigation.NavigateTo<InvoiceViewModel>();
        }

    }
}
