using System.IO;

namespace CYR.Services;

public class FileService : IFileService
{
    public List<FileModel> LoadFileNamesFromPath(string path)
    {
        List<FileModel> files = [];

        try
        {
            string[] filesFullPath = Directory.GetFiles(path);
            foreach (var file in filesFullPath)
            {
                FileModel model = new();
                model.FullPath = file;
                model.FileName = Path.GetFileName(file);
                files.Add(model);
            }
        }
        catch (Exception)
        {            
        }       
        return files;
    }
}
