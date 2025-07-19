namespace CYR.Services;

public interface IFileService
{
    List<FileModel> LoadFileNamesFromPath(string path);
}