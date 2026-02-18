using CYR.Core;
using CYR.User;
using System.Data.Common;

namespace CYR.OrderItems;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly UserContext _userContext;
    public OrderItemRepository(IDatabaseConnection databaseConnection, UserContext userContext)
    {
        _databaseConnection = databaseConnection;
        _userContext = userContext;
    }
    public async Task<bool> DeleteAsync(OrderItem orderItem)
    {
        bool succes = false;
        string query = "DELETE FROM Produkte_Dienstleistungen WHERE Produktnummer = @Produktnummer and user_id = @user_id";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "Produktnummer", orderItem.Id},
            { "user_id", _userContext.CurrentUser.Id}
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        succes = affectedRows >0;
        return succes;
    }

    public async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        List<OrderItem> orderItems = new List<OrderItem>();
        OrderItem orderItem;
        string query = $"SELECT * FROM Produkte_Dienstleistungen where user_id = {_userContext.CurrentUser.Id}";

        using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
        {
            while (await reader.ReadAsync())
            {
                orderItem = new OrderItem();
                orderItem.Id = Convert.ToInt32(reader["Produktnummer"]);
                orderItem.Description = reader["Beschreibung"].ToString();
                orderItem.Name = reader["Name"].ToString();
                orderItem.Price = reader["Preis"].ToString();
                orderItems.Add(orderItem);
            }
            return orderItems;
        }
    }

    public Task<IEnumerable<OrderItem>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InsertAsync(OrderItem orderItem)
    {
        string query = "INSERT INTO Produkte_Dienstleistungen (Name,Beschreibung,Preis,user_id) VALUES (@Name,@Beschreibung,@Preis,@user_id)";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "Name", orderItem.Name },
            { "Beschreibung", orderItem.Description },
            { "Preis", orderItem.Price },
            { "user_id", _userContext.CurrentUser.Id }
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        return affectedRows;
    }

    public async Task<bool> UpdateAsync(OrderItem orderItem)
    {
        string query = "update Produkte_Dienstleistungen  set Name = @Name, Beschreibung = @Beschreibung, Preis = @Preis where Produktnummer = @Produktnummer and user_id = @user_id";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "Produktnummer", orderItem.Id },
            { "Name", orderItem.Name },
            { "Beschreibung", orderItem.Description },
            { "Preis", orderItem.Price },
            { "user_id", _userContext.CurrentUser.Id }
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        return affectedRows > 0;
    }
}
