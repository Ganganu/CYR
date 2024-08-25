using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Services;

namespace CYR.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel(INavigationService navigationService) 
        {
            Navigation = navigationService;
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
    }
}
