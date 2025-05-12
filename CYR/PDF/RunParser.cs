using System.Xml.Linq;

namespace CYR.PDF
{
    public class RunParser
    {
        public List<Run> GetRunsAndData(string? commentsTop)
        {
            if (string.IsNullOrEmpty(commentsTop))
                return new List<Run>();

            try
            {
                XDocument doc = XDocument.Parse(commentsTop);
                                
                XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
                                
                var runElements = doc.Descendants(ns + "Run");

                return runElements.Select(runElement => new Run
                {
                    Text = runElement.Value ?? string.Empty,

                    FontSize = int.TryParse(runElement.Attribute("FontSize")?.Value, out int fontSize)
                        ? fontSize
                        : 10,

                    FontStyle = runElement.Attribute("FontStyle")?.Value,
                    FontWeight = runElement.Attribute("FontWeight")?.Value,
                    TextDecorations = runElement.Attribute("TextDecorations")?.Value
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing XML: {ex.Message}");
                return new List<Run>();
            }
        }
    }
}