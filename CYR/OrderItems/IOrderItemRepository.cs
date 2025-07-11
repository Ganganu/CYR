namespace CYR.OrderItems
{
    public interface IOrderItemRepository
    {
        Task DeleteAsync(OrderItem orderItem);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<IEnumerable<OrderItem>> GetByIdAsync(int id);
        Task InsertAsync(OrderItem orderItem);
        Task UpdateAsync(OrderItem orderItem);
    }
}