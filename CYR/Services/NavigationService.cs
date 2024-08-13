using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYR.Services
{
    public interface INavigationService
    {
        public ObservableObject CurrentView { get;  set; }
        void NavigateTo<T>() where T : ObservableObject;
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

        public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
        {
            ObservableObject viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
        }
    }
}
