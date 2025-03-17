using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Services
{
    public interface IParameterReceiver
    {
        Task ReceiveParameter(object parameter);
    }

    public interface INavigationService
    {
        public ObservableObject CurrentView { get;  set; }
        void NavigateTo<T>(object parameter = null) where T : ObservableObject;
    }

    public partial class NavigationService : ObservableObject, INavigationService
    {
        [ObservableProperty]
        private ObservableObject _currentView;
        private readonly Func<Type, ObservableObject> _viewModelFactory;

        public NavigationService(Func<Type, ObservableObject> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ObservableObject
        {
            ObservableObject viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            if (viewModel is IParameterReceiver parameterReceiver)
            {
                parameterReceiver.ReceiveParameter(parameter);
            }
            CurrentView = viewModel;
        }
    }
}
