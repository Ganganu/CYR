
namespace CYR.Clients
{
    public interface IRetrieveClients
    {
        Task<IEnumerable<Client>> Handle();
    }
}