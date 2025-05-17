using CYR.PDF;

namespace CYR.Invoice
{
    public interface IXMLService
    {
        string LoadAsync(string path);
        bool SaveAsync(string text);
    }
}