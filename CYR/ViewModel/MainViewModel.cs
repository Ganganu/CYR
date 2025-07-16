using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Dashboard.DashboardViewModels;
using CYR.Dialog;
using CYR.Invoice.InvoiceViewModels;
using CYR.Services;

namespace CYR.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel(INavigationService navigationService) 
        {
            Navigation = navigationService;
            Navigation.NavigateTo<DashboardViewModel>();
        }

        [ObservableProperty]
        private INavigationService _navigation;
                
        [RelayCommand]
        private void NavigateToClientView()
        {
            Navigation.NavigateTo<ClientViewModel>();
        }
        [RelayCommand]
        private void NavigateToArticleView()
        {
            Navigation.NavigateTo<ArticleViewModel>();
        }
        [RelayCommand]
        private void NavigateToSettingsView()
        {
            Navigation.NavigateTo<SettingsViewModel>();
        }
        [RelayCommand]
        private void NavigateToGetInvoices()
        {
            Navigation.NavigateTo<InvoiceListViewModel>();
        }
        [RelayCommand]
        private void NavigateToDashboard()
        {
            Navigation.NavigateTo<DashboardViewModel>();
        }
    }
}
