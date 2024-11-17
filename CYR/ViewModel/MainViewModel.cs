using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Dialog;
using CYR.Services;

namespace CYR.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;
        public MainViewModel(INavigationService navigationService,IDialogService dialogService) 
        {
            Navigation = navigationService;
            _dialogService = dialogService;
        }

        [ObservableProperty]
        private INavigationService _navigation;
                
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
    }
}
