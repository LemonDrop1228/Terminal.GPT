using System.Reflection;
using Newtonsoft.Json;
using TerminalGPT.Constants;
using TerminalGPT.Extensions;
using TerminalGPT.Options;

namespace TerminalGPT.Services;

public interface ISettingsService
{
    TerminalGptOptions Options { get; }
    static string SettingsPath { get; }
    void SaveSettings(TerminalGptOptions settings);
    void SetSettings(TerminalGptOptions settings);
}

public class SettingsService : ISettingsService
{
    private TerminalGptOptions _options;
    public TerminalGptOptions Options
    {
        get => _options;
        private set
        {
            if(!Equals(_options, value))
                _options = value;
        }
}
    
    public void SetSettings(TerminalGptOptions settings)
    {
        if(settings == null)
            throw new ArgumentNullException(nameof(settings));
        _options = settings;
    }
    
    public void SaveSettings(TerminalGptOptions settings)
    {
        try
        {
            File.WriteAllText(AppConstants.SettingsPath, settings.ToJson());
            Options = settings;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}