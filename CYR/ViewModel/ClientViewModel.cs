using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Invoice.InvoiceViewModels;
using CYR.Services;
using System.Collections.ObjectModel;

namespace CYR.ViewModel;

public partial class ClientViewModel : ObservableObject, IParameterReceiver
{
    private readonly IRetrieveClients _retrieveClients;
    private readonly IClientRepository _clientRepository;
    public ClientViewModel(IRetrieveClients retrieveClients, INavigationService navigationService, IClientRepository clientRepository)
    {
        _retrieveClients = retrieveClients;
        Navigation = navigationService;
        Initialize();
        _clientRepository = clientRepository;
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
        var clientsToDelete = Clients.Where(c => c.IsSelected).ToList();
        try
        {
            foreach (var client in clientsToDelete)
            {
                try
                {
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
}
