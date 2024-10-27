using CYR.Clients;
using CYR.Core;
using System.Data.Common;

namespace CYR.OrderItems
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IDatabaseConnection _databaseConnection;
        public OrderItemRepository(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }
        public Task DeleteAsync(OrderItem.OrderItem orderItem)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrderItem.OrderItem>> GetAllAsync()
        {
            List<OrderItem.OrderItem> orderItems = new List<OrderItem.OrderItem>();
            OrderItem.OrderItem orderItem;
            string query = "SELECT * FROM Produkte_Dienstleistungen";

            using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
            {
                while (await reader.ReadAsync())
                {
                    orderItem = new OrderItem.OrderItem();
                    orderItem.Id = Convert.ToInt32(reader["Produktnummer"]);
                    orderItem.Description = reader["Beschreibung"].ToString();
                    orderItem.Name = reader["Name"].ToString();
                    orderItem.Price = Convert.ToDecimal(reader["Preis"]);
                    orderItems.Add(orderItem);
                }
                return orderItems;
            }
        }

        public Task<IEnumerable<OrderItem.OrderItem>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(OrderItem.OrderItem orderItem)
        {
            string query = "INSERT INTO Produkte_Dienstleistungen (Name,Beschreibung,Preis) VALUES (@Name,@Beschreibung,@Preis)";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                { "Name", orderItem.Name },
                { "Beschreibung", orderItem.Description },
                { "Preis", orderItem.Price }
            };
            int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        }

        public Task UpdateAsync(OrderItem.OrderItem orderItem)
        {
            throw new NotImplementedException();
        }
    }
}
