using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Services;

namespace CYR.Clients.ViewModels;

public partial class UpdateClientViewModel : ObservableRecipient, IParameterReceiver
{

    public UpdateClientViewModel()
    {
        
    }


    [ObservableProperty]
    private Client? _client;
    [ObservableProperty]
    private string _clientNumber;
    [ObservableProperty]
    private string _clientName;
    [ObservableProperty]
    private string _clientTelefonnumber;
    [ObservableProperty]
    private string _clientEmail;
    [ObservableProperty]
    private string _clientCreationDate;
    [ObservableProperty]
    private string _clientStreet;
    [ObservableProperty]
    private string _clientPLZ;
    [ObservableProperty]
    private string _clientCity;
    [ObservableProperty]
    private string _errorMessage;
    public async Task ReceiveParameter(object parameter)
    {
        Client = (Client)parameter;
        ClientName = Client.Name;
        ClientTelefonnumber = Client.Telefonnumber;
        ClientEmail = Client.EmailAddress;
        ClientCreationDate = Client.CreationDate;
        ClientStreet = Client.Street;
        ClientCity = Client.City;
        ClientPLZ = Client.PLZ;
    }
}
