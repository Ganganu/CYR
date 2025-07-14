using System.Data.SQLite;
using System.Linq.Expressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Core;
using CYR.Dialog;
using CYR.Extensions;
using CYR.Services;
using CYR.ViewModel;

namespace CYR.Clients.ViewModels;

public partial class UpdateClientViewModel : ObservableRecipient, IParameterReceiver
{
    private readonly IClientRepository _clientRepository;
    private readonly IDialogService _dialogService;

    private string? _dialogResponse;
    private IEnumerable<Client> _clients;
    public UpdateClientViewModel(INavigationService navigationService, IClientRepository clientRepository, IDialogService dialogService)
    {
        Navigation = navigationService;
        _clientRepository = clientRepository;
        _dialogService = dialogService;
    }

    [ObservableProperty]
    private INavigationService _navigation;
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
        ClientNumber = Client.ClientNumber;
        ClientName = Client.Name;
        ClientTelefonnumber = Client.Telefonnumber;
        ClientEmail = Client.EmailAddress;
        ClientCreationDate = Client.CreationDate;
        ClientStreet = Client.Street;
        ClientCity = Client.City;
        ClientPLZ = Client.PLZ;
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        _clients = await _clientRepository.GetAllAsync();
        Navigation.NavigateTo<ClientViewModel>(_clients);
    }
    [RelayCommand]
    private async Task UpdateClient()
    {
        if (Client is null) return;
        bool valid = ValidateProperties();
        if (!valid)
            return;

        Client clientToUpdate = CreateUpdatedClient(Client);

        try
        {
            ShowNotificationDialog("Kunde aktualisieren", $"Möchten Sie wirklich die Daten speichern?",
            "Nein", "User", Visibility.Visible, "Ja");
            if (_dialogResponse != "True")
            {
                return;
            }
            await _clientRepository.UpdateAsync(clientToUpdate);

        }
        catch (SQLiteException ex)
        {
            SQLiteErrorCodes errorCode = (SQLiteErrorCodes)Convert.ToInt32(ex.ErrorCode);
            ErrorMessage = errorCode.GetDescription();
            return;
        }
        ShowNotificationDialog("Kunde erfolgreich aktualisiert", $"Der Kunde {ClientName} wurde erfolgreich aktualisiert.",
            "Ok", "User", Visibility.Collapsed, "");
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
        bool valid = !string.IsNullOrEmpty(ClientName) && !string.IsNullOrEmpty(ClientTelefonnumber) &&
            !string.IsNullOrEmpty(ClientCreationDate) && !string.IsNullOrEmpty(ClientStreet) && !string.IsNullOrEmpty(ClientPLZ) &&
            !string.IsNullOrEmpty(ClientCity);
        if (!valid)
            return "Unvolständige Daten!";
        else
            return string.Empty;
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
    private Client CreateUpdatedClient(Client client)
    {
        Client c = new Client();
        c.ClientNumber = client.ClientNumber;
        c.Name = ClientName;
        c.Street = ClientStreet;
        c.City = ClientCity;
        c.Street = ClientStreet;
        c.CreationDate = ClientCreationDate;
        c.EmailAddress = ClientEmail;
        c.Telefonnumber = ClientTelefonnumber;
        c.PLZ = ClientPLZ;
        return c;
    }
}
