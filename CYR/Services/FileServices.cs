using System.IO;

namespace CYR.Services
{
    public class FileService : IFileService
    {
        public FileService()
        {

        }

        public List<string> LoadFileNamesFromPath(string path)
        {
            string[] filesFullPath = Directory.GetFiles(path);
            List<string> files = new();
            foreach (var file in filesFullPath)
            {
                files.Add(Path.GetFileName(file));
            }
            return files;
        }
    }
}
