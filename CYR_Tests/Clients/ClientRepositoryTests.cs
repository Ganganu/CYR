using CYR.Address;
using CYR.Clients;
using CYR.Core;
using CYR.User;

namespace CYR.Tests.Clients;

public class ClientRepositoryTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly string _connectionString;
    private readonly UserContext _userContext;
    private readonly IDatabaseConnection _databaseConnection;
    private readonly ClientRepository _repository;
    private readonly AddressRepository _addressRepository;
    public ClientRepositoryTests()
    {
        string originalDbPath = Path.Combine(AppContext.BaseDirectory, "integration_cyr_tests.db.db");
        _testDbPath = Path.Combine(Path.GetTempPath(), $"CYR_Test_{Guid.NewGuid()}.db");
        File.Copy(originalDbPath, _testDbPath, overwrite: true);
        _connectionString = $"DataSource={_testDbPath}";
        _databaseConnection = new SQLiteConnectionManager(_connectionString);
        _userContext = new UserContext();
        _userContext.CurrentUser = new User.User();
        _userContext.CurrentUser.Id = "11";
        _userContext.CurrentUser.Logo = "";
        _userContext.CurrentUser.Username = "admin";
        _userContext.CurrentUser.Password = "admin";
        _userContext.CurrentUser.Role = "admin";
        _repository = new ClientRepository(_databaseConnection, _userContext);
        _addressRepository = new AddressRepository(_databaseConnection, _userContext);
    }
    [Fact]
    public async Task InsertAsync_ShoudInsertClientIntoDatabase()
    {
        AddressModel addressModel = new()
        {
            CompanyName = "2525",
            Street = "TestStreet",
            City = "TestCity",
            PLZ = "TestPLZ",            
        };
        await _addressRepository.InsertAsync(addressModel);

        Client model = new()
        {
            ClientNumber = "2525",
            Name = "1",
            Telefonnumber = "1",
            EmailAddress = "1"
        };
        Client result = await _repository.InsertAsync(model);

        IEnumerable<Client> clients = await _repository.GetAllAsync();
        Assert.Contains(clients, p => p.ClientNumber == "2525");
        Assert.Contains(clients, p => p.Street == "TestStreet");
        Assert.Contains(clients, p => p.PLZ == "TestPLZ");
    }

    public void Dispose()
    {
        if (File.Exists(_testDbPath)) File.Delete(_testDbPath);
    }
}
