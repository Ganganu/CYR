using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Address;
using CYR.Clients;
using CYR.Clients.ViewModels;
using CYR.Clients.Views;
using CYR.Core;
using CYR.Dashboard;
using CYR.Dashboard.DashboardViewModels;
using CYR.Dashboard.DashboardViews;
using CYR.Dialog;
using CYR.Invoice;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Invoice.InvoiceServices;
using CYR.Invoice.InvoiceViewModels;
using CYR.Invoice.InvoiceViews;
using CYR.Invoice.UseCases;
using CYR.Logging;
using CYR.Login;
using CYR.OrderItems;
using CYR.OrderItems.OrderItemCommand;
using CYR.OrderItems.OrderItemViewModels;
using CYR.OrderItems.OrderItemViews;
using CYR.Services;
using CYR.Settings;
using CYR.UnitOfMeasure;
using CYR.User;
using CYR.User.UseCases;
using CYR.View;
using CYR.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using System.Windows;

namespace CYR;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;
    private string connectionString = "Data Source=.\\cyr.db;Version=3;foreign_keys=on";
    private UserContext _userContext;
    public App()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<MainWindow>(provider => new MainWindow { DataContext = provider.GetRequiredService<MainViewModel>() });
        services.AddTransient<ClientView>();
        services.AddTransient<CreateNewArticleView>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<ClientViewModel>();
        services.AddTransient<CreateInvoiceViewModel>();
        services.AddTransient<InvoiceListViewModel>();
        services.AddTransient<ShowInvoiceViewModel>();
        services.AddTransient<ShowInvoiceView>();
        services.AddTransient<CreateNewArticleViewModel>();
        services.AddTransient<ArticleView>();
        services.AddTransient<CreateInvoiceView>();
        services.AddSingleton<InvoiceListView>();
        services.AddTransient<ArticleViewModel>();
        services.AddSingleton<CreateClientView>();
        services.AddSingleton<CreateClientViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SettingsView>();
        services.AddSingleton<RegisterView>();
        services.AddSingleton<RegisterViewModel>();
        services.AddTransient<LoginView>(provider => new LoginView { DataContext = provider.GetRequiredService<LoginViewModel>() });
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginRepository>();
        services.AddTransient<UpdateClientViewModel>();
        services.AddTransient<UpdateClientView>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<DashboardView>();
        services.AddTransient<UpdateOrderItemViewModel>();
        services.AddTransient<UpdateOrderItemView>();
        services.AddTransient<StatisticOverviewViewModel>();
        services.AddTransient<StatisticOverviewView>();
        services.AddTransient<StatisticChartView>();
        services.AddTransient<StatisticChartViewModel>();
        services.AddTransient<DashboardActivityView>();
        services.AddTransient<DashboardActivityViewModel>();
        services.AddTransient<UserView>();
        services.AddTransient<UnitOfMeasureView>();
        services.AddTransient<UnitOfMeasureViewModel>();
        services.AddTransient<CreateUnitOfMeasureView>();
        services.AddTransient<CreateUnitOfMeasureViewModel>();
        services.AddTransient<UpdateUnitOfMeasureView>();
        services.AddTransient<UpdateUnitOfMeasureViewModel>();
        services.AddTransient<UserViewModel>();
        services.AddTransient<DashboardInvoiceView>();
        services.AddTransient<DashboardInvoiceViewModel>();
        services.AddSingleton<IOrderItemRepository, OrderItemRepository>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IClientRepository, ClientRepository>();
        services.AddSingleton<IAddressRepository, AddressRepository>();
        services.AddSingleton<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
        services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
        services.AddSingleton<LoggingRepository>();

        services.AddSingleton<StatisticOverviewRepository>();
        services.AddSingleton<StatisticChartRepository>();
        services.AddSingleton<UserRepository>();
        services.AddSingleton<CompanyRepository>();
        services.AddSingleton<UserContext>();
        services.AddSingleton<UserCompanyRepository>();
        services.AddSingleton<DashboardActivityRepository>();



        services.AddSingleton<ImportOrderItemsCommand>();
        services.AddTransient<UpdateCompanyLogo>();
        services.AddTransient<GetLogoUseCase>();




        services.AddSingleton<IInvoicePositionRepository, InvoicePositionRepository>();
        services.AddSingleton<ISaveInvoiceInvoicePositionService, SaveInvoiceInvoicePositionService>();
        services.AddSingleton<IPreviewInvoiceService, PreviewInvoiceService>();
        services.AddSingleton<ISelectImageService, SelectImageService>();
        services.AddSingleton<IArticleImportService, ArticleImportService>();
        services.AddSingleton<IArticleImportMethod, CsvArticleImportMethod>();
        services.AddSingleton<IArticleImportMethod, JsonArticleImportMethod>();
        services.AddSingleton<IOpenImageService, OpenImageService>();
        services.AddSingleton<IXMLService, XMLService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IPrintOrderItemService, PrintOrderItemService>();
        services.AddSingleton<IInvoiceDocument, InvoiceDocument>();
        services.AddSingleton<Client>();
        services.AddSingleton<InvoiceModel>();
        services.AddSingleton<User.User>();
        services.AddSingleton<InvoiceDocument>();
        services.AddSingleton<IRetrieveClients, RetrieveClients>();
        services.AddTransient<IPasswordHasherService, PasswordHasherService>();
        services.AddTransient<ILoginTokenService, LoginTokenService>();
        services.AddSingleton<IDatabaseConnection, SQLiteConnectionManager>((provider) => new SQLiteConnectionManager(connectionString));
        services.AddSingleton<Func<Type, ObservableObject>>(serviceProvider => viewModelType => (ObservableObject)serviceProvider.GetRequiredService(viewModelType));
        _serviceProvider = services.BuildServiceProvider();
                
        //Dialog Registrations
        DialogService.RegisterDialog<Notification,NotificationViewModel>();
        DialogService.RegisterDialog<ErrorDialogView, ErrorDialogViewModel>();
        DialogService.RegisterDialog<ItemsListView, ItemsListDialogViewModel>(() => [new XMLService()]);
        DialogService.RegisterDialog<SaveCommentsDialogView, SaveCommentsDialogViewModel>(() => [new XMLService()]);
    }
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var loginTokenService = _serviceProvider.GetRequiredService<ILoginTokenService>();
        var loginRepository = _serviceProvider.GetRequiredService<LoginRepository>();

        var tokenData = loginTokenService.LoadToken();
        if (tokenData != null)
        {
            bool success = await loginRepository.LoginWithToken(tokenData.Value.Username, tokenData.Value.Token);
            if (success)
            {
                var userRepository = _serviceProvider.GetRequiredService<UserRepository>();
                var user = await userRepository.GetUserAsync(tokenData.Value.Username);
                _userContext = _serviceProvider.GetRequiredService<UserContext>();
                _userContext.CurrentUser = user;
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
                return;
            }
            else
            {
                loginTokenService.DeleteToken();
            }
        }

        var loginView = _serviceProvider.GetRequiredService<LoginView>();
        loginView.Show();

        DependencyPropertyChangedEventHandler handler = null;
        handler = (s, ev) =>
        {
            if (!loginView.IsVisible && loginView.IsLoaded)
            {
                loginView.IsVisibleChanged -= handler;
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
                loginView.Dispatcher.InvokeAsync(() => loginView.Close());
            }
        };
        loginView.IsVisibleChanged += handler;
    }

}
