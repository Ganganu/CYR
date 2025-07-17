using CYR.PDF;

namespace CYR.Invoice
{
    public interface IXMLService
    {
        Task<string> LoadAsync(string path);
        Task<bool> SaveAsync(string text);
    }
}