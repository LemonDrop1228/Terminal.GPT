using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace TerminalGPT.Options;

public class TerminalGptOptions : IOptions<TerminalGptOptions>
{
    public string? ApiKey { get; set; }
    public string? OrgId { get; set; }
    public GPTModel? Model { get; set; }
    public string? SystemPrompt { get; set; }
    public bool ShowSystemPrompt { get; set; }
    public bool ShowChatTitle { get; set; }
    public bool ShowAboutInfo { get; set; }
    public bool WhereArtThouTimmy { get; set; }
    [JsonIgnore]
    public TerminalGptOptions Value => this;

    public TerminalGptOptions Merge(TerminalGptOptions optionsMonitorCurrentValue)
    {
        if (string.IsNullOrEmpty(Value.ApiKey))
        {
            Value.ApiKey = optionsMonitorCurrentValue.ApiKey;
        }

        if (string.IsNullOrEmpty(Value.OrgId))
        {
            Value.OrgId = optionsMonitorCurrentValue.OrgId;
        }

        if (Value.Model == null)
        {
            Value.Model = optionsMonitorCurrentValue.Model;
        }

        if (string.IsNullOrEmpty(Value.SystemPrompt))
        {
            Value.SystemPrompt = optionsMonitorCurrentValue.SystemPrompt;
        }

        if (Value.ShowSystemPrompt == false)
        {
            Value.ShowSystemPrompt = optionsMonitorCurrentValue.ShowSystemPrompt;
        }

        if (Value.ShowChatTitle == false)
        {
            Value.ShowChatTitle = optionsMonitorCurrentValue.ShowChatTitle;
        }

        if (Value.ShowAboutInfo == false)
        {
            Value.ShowAboutInfo = optionsMonitorCurrentValue.ShowAboutInfo;
        }

        return Value;
    }

    public bool Validate()
    {
        if (string.IsNullOrEmpty(ApiKey) | string.IsNullOrEmpty(OrgId) | Model == null)
            return false;

        return true;
    }
}

public enum GPTModel
{
    GPT4,
    GPT4_32k
}