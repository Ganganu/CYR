using CYR.Address;
using CYR.Clients;
using CYR.Core;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.User;

namespace CYR.Tests.Invoice.InvoiceRepositorys;

public class InvoiceRepositoryTests : IDisposable
{
    private readonly string _testDbPath;
    private readonly string _connectionString;
    private readonly UserContext _userContext;
    private readonly IDatabaseConnection _databaseConnection;
    private readonly InvoiceRepository _invoiceRepository;
    private readonly AddressRepository _addressRepository;
    private readonly ClientRepository _clientRepository;

    public InvoiceRepositoryTests()
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
        _invoiceRepository = new InvoiceRepository(_databaseConnection, _userContext);
        _addressRepository = new AddressRepository(_databaseConnection, _userContext);
        _clientRepository = new ClientRepository(_databaseConnection, _userContext);
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertInvoiceIntoDatabase()
    {
        await InsertInvoice();
        IEnumerable<InvoiceModel> invoiceModels = await _invoiceRepository.GetAllAsync();
        Assert.Contains(invoiceModels, i => i.InvoiceNumber == 1001);
    }
    [Fact]
    public async Task DeleteAsync_ShouldDeleteInvoiceFromDatabase()
    {
        InvoiceModel invoiceModel = await InsertInvoice();
        IEnumerable<InvoiceModel> invoiceModels = await _invoiceRepository.GetAllAsync();
        Assert.Contains(invoiceModels, i => i.InvoiceNumber == 1001);
        await _invoiceRepository.DeleteAsync(invoiceModel);
        invoiceModels = await _invoiceRepository.GetAllAsync();
        Assert.Empty(invoiceModels);
    }

    private async Task<InvoiceModel> InsertInvoice()
    {
        AddressModel addressModel = new()
        {
            CompanyName = "2525",
            Street = "TestStreet",
            City = "TestCity",
            PLZ = "TestPLZ",
        };
        await _addressRepository.InsertAsync(addressModel);

        Client clientModel = new()
        {
            ClientNumber = "2525",
            Name = "1",
            Telefonnumber = "1",
            EmailAddress = "1"
        };
        await _clientRepository.InsertAsync(clientModel);

        InvoiceModel invoiceModel = new()
        {
            InvoiceNumber = 1001,
            Customer = clientModel,
            IssueDate = new DateTime(2025, 1, 1),
            DueDate = new DateTime(2025, 2, 1),
            NetAmount = 25,
            GrossAmount = 55,
            State = InvoiceState.Open,
            CommentsTop = "Top",
            CommentsBottom = "Bottom"
        };
        await _invoiceRepository.InsertAsync(invoiceModel);
        return invoiceModel;
    }

    public void Dispose()
    {
        if (File.Exists(_testDbPath)) File.Delete(_testDbPath);
    }
}
