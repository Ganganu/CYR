namespace CYR.Services
{
    public interface IFileService
    {
        List<string> LoadFileNamesFromPath(string path);
    }
}