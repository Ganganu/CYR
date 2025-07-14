using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Clients.ViewModels;
using CYR.Dialog;
using CYR.Invoice.InvoiceViewModels;
using CYR.Services;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.ViewModel;

public partial class ClientViewModel : ObservableObject, IParameterReceiver
{
    private readonly IRetrieveClients _retrieveClients;
    private readonly IClientRepository _clientRepository;
    private readonly IDialogService _dialogService;

    private string? _dialogResponse;

    public ClientViewModel(IRetrieveClients retrieveClients, INavigationService navigationService, IClientRepository clientRepository, IDialogService dialogService)
    {
        _retrieveClients = retrieveClients;
        Navigation = navigationService;
        Initialize();
        _clientRepository = clientRepository;
        _dialogService = dialogService;
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
        try
        {
            foreach (var client in clientsToDelete)
            {
                try
                {
                    ShowNotificationDialog("Kunde löschen", $"Möchten Sie wirklich den Kunden löschen? Das würde bedeuten, dass alle Rechnungen und Daten im Zusammenhang mit diesen Kudnen auch gelsöcht werden!",
                    "Nein", "User", Visibility.Visible, "Ja");
                    if (_dialogResponse != "True")
                    {
                        return;
                    }
                    var c = await _clientRepository.DeleteAsync(client);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            foreach (var client in clientsToDelete)
            {
                Clients.Remove(client);
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

        var clientToUpdate = Clients.First(c => c.IsSelected);
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
}
