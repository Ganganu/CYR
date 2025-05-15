using CYR.PDF;

namespace CYR.Invoice
{
    public interface IXMLService
    {
        Run LoadAsync();
        bool SaveAsync(string text);
    }
}