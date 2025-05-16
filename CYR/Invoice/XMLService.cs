using System.IO;
using CYR.PDF;

namespace CYR.Invoice
{
    public class XMLService : IXMLService
    {
        private readonly string _directoryPath = AppDomain.CurrentDomain.BaseDirectory;
        public bool SaveAsync(string text)
        {
            string commentsPath = $@"{_directoryPath}\Comments";
            if (!Directory.Exists(commentsPath))
            {
                Directory.CreateDirectory(commentsPath);                
            }
            if (Directory.Exists(commentsPath))
            {
                File.WriteAllText($@"{commentsPath}\test.xml", text);
                return true;
            }
            return false;
        }
        public string LoadAsync()
        {
            string commentsPath = $@"{_directoryPath}\Comments";
            string xmlText = File.ReadAllText($@"{commentsPath}\test.xml");
            return xmlText;
        }
    }
}
