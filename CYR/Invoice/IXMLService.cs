namespace CYR.Invoice;

public interface IXMLService
{
    Task<string> LoadAsync(string filePath, string folderPath);
    Task<bool> SaveAsync(string text, string childFodler, string fileName);
}