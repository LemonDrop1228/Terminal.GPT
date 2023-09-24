using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TerminalGPT.Data;
using TerminalGPT.Enums;
using TerminalGPT.Options;

namespace TerminalGPT.Services;

public interface IChatCommandService
{
    bool TryParseCommand(string input, out ChatCommand command);
    Task ExecuteCommand(ChatCommand command);
    Task InitializeCommands();
}

public class ChatCommandService : IChatCommandService
{
    private readonly TerminalGptOptions _options;
    private readonly IServiceProvider _serviceProvider;
    
    private Lazy<IChatService> _chatService;
    private Lazy<IOpenAIService> _openAiService;
    private Lazy<IExitService> _exitService;
    private Lazy<IUserService> _userService;
    private Lazy<IMenuService> _menuService;
    private Lazy<ITerminalChatService> _terminalChatService;
    
    public IChatService ChatService => _chatService.Value;
    public IOpenAIService OpenAiService => _openAiService.Value;
    public IExitService ExitService => _exitService.Value;
    public IUserService UserService => _userService.Value;
    public IMenuService MenuService => _menuService.Value;
    public ITerminalChatService TerminalChatService => _terminalChatService.Value;

    private readonly Dictionary<CommandEnum.Command, Func<Task>> _commandDictionary = new Dictionary<CommandEnum.Command, Func<Task>>();
    
    public ChatCommandService(IOptions<TerminalGptOptions> options, IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        
        _chatService = new Lazy<IChatService>(() => _serviceProvider.GetRequiredService<IChatService>());
        _openAiService = new Lazy<IOpenAIService>(() => _serviceProvider.GetRequiredService<IOpenAIService>());
        _exitService = new Lazy<IExitService>(() => _serviceProvider.GetRequiredService<IExitService>());
        _userService = new Lazy<IUserService>(() => _serviceProvider.GetRequiredService<IUserService>());
        _menuService = new Lazy<IMenuService>(() => _serviceProvider.GetRequiredService<IMenuService>());
        _terminalChatService = new Lazy<ITerminalChatService>(() => _serviceProvider.GetRequiredService<ITerminalChatService>());
    }

    public async Task InitializeCommands()
    {
        foreach (var command in Enum.GetValues(typeof(CommandEnum.Command)).Cast<CommandEnum.Command>())
        {
            _commandDictionary.Add(command, command switch
            {
                CommandEnum.Command.Exit => async () => await ExitService.Exit(),
                CommandEnum.Command.Save => async () => await ChatService.Save(),
                _ => async () => TerminalChatService.PrintCommandNotImplemented()
            });
        }
    }

    public bool TryParseCommand(string input, out ChatCommand command)
    {
        command = null;
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        if (input[0] != '/')
        {
            return false;
        }

        var commandString = input.Split(' ')[0].Substring(1);
        var commandEnum = Enum.TryParse<CommandEnum.Command>(commandString, true, out var commandEnumResult)
            ? commandEnumResult
            : CommandEnum.Command.Unknown;
        command = new ChatCommand(commandEnum);
        
        if (command.CommandEnum == CommandEnum.Command.Unknown)
        {
            return false;
        }
        
        return true;
    }

    public async Task ExecuteCommand(ChatCommand command)
    {
        if (_commandDictionary.ContainsKey(command.CommandEnum))
        {
            await _commandDictionary[command.CommandEnum]();
        }
    }
}