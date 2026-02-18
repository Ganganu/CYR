namespace CYR.OrderItems;

public interface IOrderItemRepository
{
    Task<bool> DeleteAsync(OrderItem orderItem);
    Task<IEnumerable<OrderItem>> GetAllAsync();
    Task<IEnumerable<OrderItem>> GetByIdAsync(int id);
    Task<int> InsertAsync(OrderItem orderItem);
    Task<bool> UpdateAsync(OrderItem orderItem);
}