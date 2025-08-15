using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Address;
using CYR.Clients;
using CYR.Core;
using CYR.Dialog;
using CYR.Extensions;
using CYR.Logging;
using CYR.Messages;
using CYR.Services;
using CYR.User;

namespace CYR.ViewModel;

public partial class CreateClientViewModel : ObservableRecipientWithValidation
{
    private readonly IClientRepository _clientRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IDialogService _dialogService;
    private readonly LoggingRepository _loggingRepository;
    private readonly UserContext _userContext;

    private string? _dialogResponse;
    private IEnumerable<Client> _clients;
    public CreateClientViewModel(INavigationService navigationService, IClientRepository clientRepository, IAddressRepository addressRepository, IDialogService dialogService, LoggingRepository loggingRepository, UserContext userContext)
    {
        Navigation = navigationService;
        _clientRepository = clientRepository;
        _addressRepository = addressRepository;
        _dialogService = dialogService;
        _loggingRepository = loggingRepository;
        _userContext = userContext;
    }

    [ObservableProperty]
    private INavigationService _navigation;

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

    [RelayCommand]
    private async Task NavigateBack()
    {
        _clients = await _clientRepository.GetAllAsync();
        Navigation.NavigateTo<ClientViewModel>(_clients);
    }
    [RelayCommand]
    private async Task SaveClient()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        Client client = new Client();
        AddressModel address = new AddressModel();
        client.ClientNumber = ClientNumber;
        client.Name = ClientName;
        client.Telefonnumber = ClientTelefonnumber;
        client.EmailAddress = ClientEmail;
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
        await _loggingRepository.InsertAsync(CreateHisModel(client));
        await _addressRepository.InsertAsync(address);
        Messenger.Send(new SnackbarMessage($"Der Kunde {ClientNumber}-{ClientName} wurde erfolgreich gespeichert.", "Check"));
    }

    private HisModel CreateHisModel(Client client)
    {
        HisModel model = new HisModel();
        model.LoggingType = LoggingType.ClientCreated;
        model.ClientId = client.ClientNumber;
        model.UserId = _userContext.CurrentUser.Id;
        model.Message = $@"Client: {client.ClientNumber} wurder vom User: {_userContext.CurrentUser.Id} erstellt.";
        return model;
    }  
}
