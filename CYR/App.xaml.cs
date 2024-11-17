using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Address;
using CYR.Clients;
using CYR.Core;
using CYR.Dialog;
using CYR.Invoice;
using CYR.Model;
using CYR.OrderItems;
using CYR.Services;
using CYR.Settings;
using CYR.UnitOfMeasure;
using CYR.View;
using CYR.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
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
            QuestPDF.Settings.License = LicenseType.Community;
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>(provider => new MainWindow { DataContext = provider.GetRequiredService<MainViewModel>() });
            services.AddSingleton<ClientView>();
            services.AddSingleton<InvoiceView>();
            services.AddSingleton<CreateNewArticleView>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ClientViewModel>();
            services.AddSingleton<CreateInvoiceViewModel>();
            services.AddSingleton<InvoiceViewModel>();
            services.AddTransient<GetInvoiceViewModel>();
            services.AddSingleton<ShowInvoiceViewModel>();
            services.AddSingleton<CreateNewArticleViewModel>();
            services.AddSingleton<ArticleView>();
            services.AddSingleton<InvoiceView>();
            services.AddSingleton<CreateInvoiceView>();
            services.AddSingleton<GetInvoiceView>();
            services.AddSingleton<ArticleViewModel>();
            services.AddSingleton<CreateClientView>();
            services.AddSingleton<CreateClientViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<SettingsView>();
            services.AddSingleton<IOrderItemRepository, OrderItemRepository>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IAddressRepository, AddressRepository>();
            services.AddSingleton<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
            services.AddSingleton<IInvoicePositionRepository, InvoicePositionRepository>();
            services.AddSingleton<Client>();
            services.AddSingleton<User.User>();
            services.AddSingleton<UserSettings>();
            services.AddSingleton<InvoiceDocument>();
            services.AddSingleton<IRetrieveClients, RetrieveClients>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IDatabaseConnection, SQLLiteConnection>((provider) => new SQLLiteConnection(connectionString));
            services.AddSingleton<Func<Type, ObservableObject>>(serviceProvider => viewModelType => (ObservableObject)serviceProvider.GetRequiredService(viewModelType));
            _serviceProvider = services.BuildServiceProvider();

            IConfigurationService configService = _serviceProvider.GetRequiredService<IConfigurationService>();
            UserSettings settings = configService.GetUserSettings();

            //Dialog Registrations
            DialogService.RegisterDialog<Notification,NotificationViewModel>();
            DialogService.RegisterDialog<ErrorDialogView, ErrorDialogViewModel>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
