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
        public ViewModelBase CurrentView { get;  set; }
        void NavigateTo<T>() where T : ViewModelBase;
    }

    public class NavigationService : ObservableObject, INavigationService
    {
        private ViewModelBase _currentView;
        private readonly Func<Type, ViewModelBase> _viewModelFactory;

        public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }


        public ViewModelBase CurrentView 
        { 
            get => _currentView;
            set 
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            ViewModelBase viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
        }
    }
}
