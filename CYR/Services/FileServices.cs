using System.IO;

namespace CYR.Services
{
    public class FileService : IFileService
    {
        public FileService()
        {

        }

        public List<FileModel> LoadFileNamesFromPath(string path)
        {
            string[] filesFullPath = Directory.GetFiles(path);
            List<FileModel> files = new();
            foreach (var file in filesFullPath)
            {
                FileModel model = new FileModel();
                model.FullPath = file;
                model.FileName = Path.GetFileName(file);
                files.Add(model);
            }
            return files;
        }
    }
}
