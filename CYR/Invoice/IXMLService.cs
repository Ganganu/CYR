using CYR.PDF;

namespace CYR.Invoice
{
    public interface IXMLService
    {
        string LoadAsync();
        bool SaveAsync(string text);
    }
}