using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Address;
using CYR.Clients;
using CYR.Services;

namespace CYR.ViewModel
{
    public partial class CreateClientViewModel : ObservableObject
    {
        public CreateClientViewModel(INavigationService navigationService, IClientRepository clientRepository,IAddressRepository addressRepository)
        {
            Navigation = navigationService;
            this._clientRepository = clientRepository;
            this._addressRepository = addressRepository;
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
        private readonly IClientRepository _clientRepository;
        private readonly IAddressRepository _addressRepository;

        [RelayCommand]
        private void NavigateBack()
        {
            Navigation.NavigateTo<ClientViewModel>();
        }
        [RelayCommand]
        private void SaveClient()
        {
            Client client = new Client();
            Address.Address address = new Address.Address();
            client.ClientNumber = ClientNumber;
            client.Name = ClientName;
            client.Telefonnumber = ClientTelefonnumber;
            client.EmailAddress = ClientEmail;
            client.CreationDate = ClientCreationDate;
            address.CompanyName = ClientNumber;
            address.Street = ClientStreet;
            address.City = ClientCity;
            address.PLZ = ClientPLZ;
            
            _clientRepository.InsertAsync(client);
            _addressRepository.InsertAsync(address);
            _clientRepository.GetAllAsync();
        }
    }
}
