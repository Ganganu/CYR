using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Linq.Expressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Dialog;
using CYR.Extensions;
using CYR.Logging;
using CYR.Messages;
using CYR.Services;
using CYR.User;
using CYR.ViewModel;

namespace CYR.Clients.ViewModels;

public partial class UpdateClientViewModel : ObservableRecipientWithValidation, IParameterReceiver
{
    private readonly IClientRepository _clientRepository;
    private readonly IDialogService _dialogService;
    private readonly LoggingRepository _loggingRepository;
    private readonly UserContext _userContext;

    private string? _dialogResponse;
    private IEnumerable<Client> _clients;
    public UpdateClientViewModel(INavigationService navigationService, IClientRepository clientRepository, IDialogService dialogService, LoggingRepository loggingRepository, UserContext userContext)
    {
        Navigation = navigationService;
        _clientRepository = clientRepository;
        _dialogService = dialogService;
        _loggingRepository = loggingRepository;
        _userContext = userContext;
    }

    [ObservableProperty]
    private INavigationService _navigation;
    [ObservableProperty]
    private Client? _client;
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string _clientNumber;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string _clientName;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    [RegularExpression(@"^\s*(?:\+?\d{1,3}[\s.-]?)?(?:\(?\d{1,4}\)?[\s.-]?){2,5}(?:\s*(?:ext\.?|x)\s*\d{1,5})?\s*$",
        ErrorMessage = "Ungültiges Format.")]
    private string _clientTelefonnumber;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$",
                   ErrorMessage = "Ungültiges Format.")]
    private string _clientEmail;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string _clientStreet;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
    private string _clientPLZ;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Feld darf nicht leer sein.")]
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
        Messenger.Send(new SnackbarMessage($"Der Kunde {ClientName} wurde erfolgreich aktualisiert.", "Check"));
        await _loggingRepository.InsertAsync(CreateHisModel());
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
            !string.IsNullOrEmpty(ClientStreet) && !string.IsNullOrEmpty(ClientPLZ) &&
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
        c.EmailAddress = ClientEmail;
        c.Telefonnumber = ClientTelefonnumber;
        c.PLZ = ClientPLZ;
        return c;
    }
    private HisModel CreateHisModel()
    {
        HisModel hisModel = new();
        hisModel.LoggingType = LoggingType.UserUpdated;
        hisModel.ClientId = ClientNumber;
        hisModel.UserId = _userContext.CurrentUser.Id;
        hisModel.Message = @$"Client:{hisModel.ClientId} wurde geupdated vom User:{hisModel.UserId}";
        return hisModel;
    }
}
