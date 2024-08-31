





namespace CYR.OrderItems
{
    public interface IOrderItemRepository
    {
        Task DeleteAsync(OrderItem.OrderItem orderItem);
        Task<IEnumerable<OrderItem.OrderItem>> GetAllAsync();
        Task<IEnumerable<OrderItem.OrderItem>> GetByIdAsync(int id);
        Task InsertAsync(OrderItem.OrderItem orderItem);
        Task UpdateAsync(OrderItem.OrderItem orderItem);
    }
}