using Microsoft.Extensions.Options;

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
    public TerminalGptOptions Value => this;
}

public enum GPTModel
{
    GPT4,
    GPT4_32k
}