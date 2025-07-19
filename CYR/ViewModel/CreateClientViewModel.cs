using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Address;
using CYR.Clients;
using CYR.Core;
using CYR.Dialog;
using CYR.Extensions;
using CYR.Messages;
using CYR.Services;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Windows;

namespace CYR.ViewModel;

public partial class CreateClientViewModel : ObservableRecipient
{
    private readonly IClientRepository _clientRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IDialogService _dialogService;

    private string? _dialogResponse;
    private IEnumerable<Client> _clients;
    public CreateClientViewModel(INavigationService navigationService, IClientRepository clientRepository, IAddressRepository addressRepository, IDialogService dialogService)
    {
        Navigation = navigationService;
        _clientRepository = clientRepository;
        _addressRepository = addressRepository;
        _dialogService = dialogService;
    }

    [ObservableProperty]
    private INavigationService _navigation;
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

    [RelayCommand]
    private async Task NavigateBack()
    {
        _clients = await _clientRepository.GetAllAsync();
        Navigation.NavigateTo<ClientViewModel>(_clients);
    }
    [RelayCommand]
    private async Task SaveClient()
    {
        bool valid = ValidateProperties();
        if (!valid)
            return;

        Client client = new Client();
        AddressModel address = new AddressModel();
        client.ClientNumber = ClientNumber;
        client.Name = ClientName;
        client.Telefonnumber = ClientTelefonnumber;
        client.EmailAddress = ClientEmail;
        client.CreationDate = ClientCreationDate;
        address.CompanyName = ClientNumber;
        address.Street = ClientStreet;
        address.City = ClientCity;
        address.PLZ = ClientPLZ;

        try
        {
            await _clientRepository.InsertAsync(client);
        }
        catch (SQLiteException ex)
        {
            SQLiteErrorCodes errorCode = (SQLiteErrorCodes)Convert.ToInt32(ex.ErrorCode);
            ErrorMessage = errorCode.GetDescription();
            return;
        }
        await _addressRepository.InsertAsync(address);
        Messenger.Send(new SnackbarMessage($"Der Kunde {ClientNumber}-{ClientName} wurde erfolgreich gespeichert.", "Check"));
    }

    private bool ValidateProperties()
    {
        bool isProeprtiesValid = true;
        string message = MessageIsNullOrEmpty();
        if (message != string.Empty)
            isProeprtiesValid = false;                
        ErrorMessage = message;
        return isProeprtiesValid;
    }
    private string MessageIsNullOrEmpty()
    {
        bool valid = !string.IsNullOrEmpty(ClientNumber) && !string.IsNullOrEmpty(ClientName) && !string.IsNullOrEmpty(ClientTelefonnumber) &&
            !string.IsNullOrEmpty(ClientCreationDate) && !string.IsNullOrEmpty(ClientStreet) && !string.IsNullOrEmpty(ClientPLZ) &&
            !string.IsNullOrEmpty(ClientCity);
        if (!valid)
            return "Unvolständige Daten!";
        else
            return string.Empty;
    }    
}
