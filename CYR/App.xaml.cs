using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Clients;
using CYR.Core;
using CYR.Services;
using CYR.View;
using CYR.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CYR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        private string connectionString = "Data Source=.\\cyr.db;Version=3;foreign_keys=on";
        public App() 
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>(provider => new MainWindow { DataContext = provider.GetRequiredService<MainViewModel>() });
            services.AddSingleton<ClientView>();
            services.AddSingleton<InvoiceView>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ClientViewModel>();
            services.AddSingleton<InvoiceViewModel>();
            services.AddSingleton<INavigationService,NavigationService>();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<Client>();
            services.AddSingleton<IRetrieveClients,RetrieveClients>();
            services.AddSingleton<IDatabaseConnection, SQLLiteConnection>((provider) => new SQLLiteConnection(connectionString));
            services.AddSingleton<Func<Type, ObservableObject>>(serviceProvider => viewModelType => (ObservableObject)serviceProvider.GetRequiredService(viewModelType));
            _serviceProvider = services.BuildServiceProvider();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
