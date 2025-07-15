using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Address;
using CYR.Clients;
using CYR.Clients.ViewModels;
using CYR.Clients.Views;
using CYR.Core;
using CYR.Dialog;
using CYR.Invoice;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Invoice.InvoiceServices;
using CYR.Invoice.InvoiceViewModels;
using CYR.Invoice.InvoiceViews;
using CYR.OrderItems;
using CYR.OrderItems.OrderItemViewModels;
using CYR.OrderItems.OrderItemViews;
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
            services.AddSingleton<CreateNewArticleView>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ClientViewModel>();
            services.AddTransient<CreateInvoiceViewModel>();
            services.AddTransient<InvoiceListViewModel>();
            services.AddTransient<ShowInvoiceViewModel>();
            services.AddTransient<ShowInvoiceView>();
            services.AddSingleton<CreateNewArticleViewModel>();
            services.AddTransient<ArticleView>();
            services.AddTransient<CreateInvoiceView>();
            services.AddSingleton<InvoiceListView>();
            services.AddTransient<ArticleViewModel>();
            services.AddSingleton<CreateClientView>();
            services.AddSingleton<CreateClientViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<SettingsView>();
            services.AddSingleton<UpdateClientViewModel>();
            services.AddSingleton<UpdateClientView>();
            services.AddSingleton<UpdateOrderItemViewModel>();
            services.AddSingleton<UpdateOrderItemView>();
            services.AddSingleton<IOrderItemRepository, OrderItemRepository>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IAddressRepository, AddressRepository>();
            services.AddSingleton<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
            services.AddSingleton<IInvoicePositionRepository, InvoicePositionRepository>();
            services.AddSingleton<ISaveInvoiceInvoicePositionService, SaveInvoiceInvoicePositionService>();
            services.AddSingleton<IPreviewInvoiceService, PreviewInvoiceService>();
            services.AddSingleton<ISelectImageService, SelectImageService>();
            services.AddSingleton<IOpenImageService, OpenImageService>();
            services.AddSingleton<IXMLService, XMLService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IInvoiceDocument, InvoiceDocument>();
            services.AddSingleton<Client>();
            services.AddSingleton<InvoiceModel>();
            services.AddSingleton<User.User>();
            services.AddSingleton<UserSettings>();
            services.AddSingleton<InvoiceDocument>();
            services.AddSingleton<IRetrieveClients, RetrieveClients>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IDatabaseConnection, SQLiteConnectionManager>((provider) => new SQLiteConnectionManager(connectionString));
            services.AddSingleton<Func<Type, ObservableObject>>(serviceProvider => viewModelType => (ObservableObject)serviceProvider.GetRequiredService(viewModelType));
            _serviceProvider = services.BuildServiceProvider();

            IConfigurationService configService = _serviceProvider.GetRequiredService<IConfigurationService>();
            UserSettings settings = configService.GetUserSettings();

            //Dialog Registrations
            DialogService.RegisterDialog<Notification,NotificationViewModel>();
            DialogService.RegisterDialog<ErrorDialogView, ErrorDialogViewModel>();
            DialogService.RegisterDialog<ItemsListView, ItemsListDialogViewModel>(() => [new XMLService()]);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
