using CYR.Settings;

namespace CYR.Services;

public interface IConfigurationService
{
    UserSettings GetUserSettings();
    void SaveSettings();
}