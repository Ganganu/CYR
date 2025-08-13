using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Clients;
using CYR.Clients.ViewModels;
using CYR.Core;
using CYR.Dialog;
using CYR.Invoice.InvoiceViewModels;
using CYR.Logging;
using CYR.Services;
using CYR.User;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.ViewModel;

public partial class ClientViewModel : ObservableRecipient, IParameterReceiver
{
    private readonly IRetrieveClients _retrieveClients;
    private readonly IClientRepository _clientRepository;
    private readonly IDialogService _dialogService;
    private readonly LoggingRepository _loggingRepository;
    private readonly UserContext _userContext;

    private string? _dialogResponse;

    public ClientViewModel(IRetrieveClients retrieveClients, INavigationService navigationService, IClientRepository clientRepository, IDialogService dialogService, LoggingRepository loggingRepository, UserContext userContext)
    {
        _retrieveClients = retrieveClients;
        Navigation = navigationService;
        Initialize();
        _clientRepository = clientRepository;
        _dialogService = dialogService;
        _loggingRepository = loggingRepository;
        _userContext = userContext;
    }

    private async void Initialize()
    {
        IEnumerable<Client> cl = await _retrieveClients.Handle();
        Clients = new ObservableCollection<Client>(cl);
    }
    [ObservableProperty]
    private INavigationService _navigation;
    [ObservableProperty]
    private ObservableCollection<Client>? _clients;

    [RelayCommand]
    private void NavigateBack()
    {
        Messenger.Send(new NavigateBackSource(typeof(ClientViewModel)));
    }

    [RelayCommand]
    public void CreateInvoice(Client client)
    {
        Navigation.NavigateTo<CreateInvoiceViewModel>(client);
    }
    [RelayCommand]
    private void CreateNewClient()
    {
        Navigation.NavigateTo<CreateClientViewModel>();
    }

    public async Task ReceiveParameter(object parameter)
    {
        if (parameter != null)
        {
            ObservableCollection<Client> cl = new ObservableCollection<Client>((List<Client>)parameter);
            if (cl != null)
            {
                Clients = cl;
            }
        }
    }
    [RelayCommand]
    private async Task DeleteClient()
    {
        if (Clients is null || !Clients.Any())
            return;

        var clientsToDelete = Clients.Where(c => c.IsSelected).ToList();
        if (clientsToDelete.Count == 0) return;
        try
        {
            foreach (var client in clientsToDelete)
            {
                try
                {
                    ShowNotificationDialog("Kunde löschen", $"Möchten Sie wirklich den Kunden {client.ClientNumber} löschen? Das würde bedeuten, dass alle Rechnungen und Daten im Zusammenhang mit diesen Kudnen auch gelsöcht werden!",
                    "Nein", "User", Visibility.Visible, "Ja");
                    if (_dialogResponse != "True")
                    {
                        return;
                    }
                    var c = await _clientRepository.DeleteAsync(client);
                    Clients.Remove(client);
                    await _loggingRepository.InsertAsync(CreateHisModel(client));
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    [RelayCommand]
    private async Task UpdateClient()
    {
        if (Clients is null || !Clients.Any())
            return;

        var clientToUpdate = Clients.FirstOrDefault(c => c.IsSelected);
        if (clientToUpdate is null) return;
        Navigation.NavigateTo<UpdateClientViewModel>(clientToUpdate);
    }
    private void ShowNotificationDialog(string title,
            string message,
            string cancelButtonText,
            string icon,
            Visibility okButtonVisibility, string okButtonText)
    {
        _dialogService.ShowDialog(result =>
        {
            _dialogResponse = result;
        },
        new Dictionary<Expression<Func<NotificationViewModel, object>>, object>
        {
                { vm => vm.Title, title },
                { vm => vm.Message,  message},
                { vm => vm.CancelButtonText, cancelButtonText },
                { vm => vm.Icon,icon },
                { vm => vm.OkButtonText, okButtonText },
                { vm => vm.IsOkVisible, okButtonVisibility}
        });
    }

    private HisModel CreateHisModel(Client client)
    {
        HisModel hisModel = new();
        hisModel.LoggingType = LoggingType.UserDeleted;
        hisModel.ClientId = client.ClientNumber;
        hisModel.UserId = _userContext.CurrentUser.Id;
        hisModel.Message = @$"Client:{hisModel.ClientId} wurde gelöscht vom User:{hisModel.UserId}";
        return hisModel;
    }
}
