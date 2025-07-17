using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace CYR.Invoice;

public class XMLService : IXMLService
{
    private readonly string _directoryPath = AppDomain.CurrentDomain.BaseDirectory;
    public async Task<bool> SaveAsync(string text, string childFolder, string fileName)
    {
        string commentsPath = $@"{_directoryPath}\Comments\{childFolder}";
        if (!Directory.Exists(commentsPath))
        {
            Directory.CreateDirectory(commentsPath);                
        }
        if (Directory.Exists(commentsPath))
        {
            await File.WriteAllTextAsync($@"{commentsPath}\{fileName}.xml", text);
            return true;
        }
        return false;
    }
    public async Task<string> LoadAsync(string path)
    {
        string commentsPath = $@"{_directoryPath}\Comments";
        string xmlText = await File.ReadAllTextAsync($@"{commentsPath}\{path}");
        string convertedXml = ConvertSectionToFlowDocument(xmlText);
        return convertedXml;
    }
    private static string ConvertSectionToFlowDocument(string xmlContent)
    {
        var stringReader = new StringReader(xmlContent);
        var xmlReader = XmlReader.Create(stringReader);
        Section section = (Section)XamlReader.Load(xmlReader);

        FlowDocument flowDoc = new();
        flowDoc.Blocks.Add(section);

        string xamlString = XamlWriter.Save(flowDoc);
        return xamlString;
    }
}
