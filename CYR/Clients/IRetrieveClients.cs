
namespace CYR.Clients
{
    public interface IRetrieveClients
    {
        Task<Client> Handle();
    }
}