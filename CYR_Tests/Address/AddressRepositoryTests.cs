using CYR.Address;
using CYR.Core;
using CYR.User;

namespace CYR.Tests.Address;
public class AddressRepositoryTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly string _connectionString;
    private readonly UserContext _userContext;
    private readonly IDatabaseConnection _databaseConnection;
    private readonly AddressRepository _repository;

    public AddressRepositoryTests()
    {
        string originalDbPath = Path.Combine(AppContext.BaseDirectory, "integration_cyr_tests.db");
        _testDbPath = Path.Combine(Path.GetTempPath(), $"CYR_Test_{Guid.NewGuid()}.db");
        File.Copy(originalDbPath, _testDbPath, overwrite: true);
        _connectionString = $"DataSource={_testDbPath}";
        _databaseConnection = new SQLiteConnectionManager(_connectionString);
        _userContext = new UserContext();
        _userContext.CurrentUser = new User.User();
        _userContext.CurrentUser.Id = "1";
        _userContext.CurrentUser.Logo = "";
        _userContext.CurrentUser.Username = "admin";
        _userContext.CurrentUser.Password = "admin";
        _userContext.CurrentUser.Role = "admin";
        _repository = new AddressRepository(_databaseConnection, _userContext);
    }

    [Fact]
    public async Task  InsertAsync_ShoudInsertAddressIntoDatabase()
    {
        AddressModel model = new()
        {
            Street = "TestStreet",
            Email = "testemail@email.com",
            CompanyName = "TestCompanyName",
            City = "TestCity",
            Phone = "TestPhone",
            State = "TestState",
            PLZ = "TestPLZ"
        };
        await _repository.InsertAsync(model);

        IEnumerable<AddressModel> persons = await _repository.GetAllAsync();
        Assert.Contains(persons, p => p.CompanyName == "TestCompanyName");
    }

    public void Dispose()
    {
        if (File.Exists(_testDbPath)) File.Delete(_testDbPath);
    }
}
