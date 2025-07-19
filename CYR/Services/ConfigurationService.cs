using System.Configuration;
using CYR.Settings;

namespace CYR.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly Configuration _configuration;
    private readonly UserSettings _userSettings;

    public ConfigurationService()
    {
        try
        {
            _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            _userSettings = (UserSettings)_configuration.Sections["UserSettings"];
            if (_userSettings == null)
            {
                _userSettings = new UserSettings();
                _configuration.Sections.Add("UserSettings", _userSettings);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Configuration Error: {ex.Message}");
            throw;
        }
    }

    public UserSettings GetUserSettings()
    {
        return _userSettings;
    }

    public void SaveSettings()
    {
        try
        {
            _configuration.Sections["UserSettings"].SectionInformation.ForceSave = true;

            _configuration.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("UserSettings");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
            throw;
        }
    }
}