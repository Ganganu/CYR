namespace CYR.Services;

public interface IArticleImportMethod
{
    string Method { get; }
    string Import(string fileName);

}
