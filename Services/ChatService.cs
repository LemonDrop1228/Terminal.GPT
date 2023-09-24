using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI.Models;
using Spectre.Console;
using TerminalGPT.Options;
using Newtonsoft.Json;

namespace TerminalGPT.Services;

public interface IChatService
{
    ObservableCollection<ChatThread> ChatThreads { get; set; }
    ChatThread? CurrentThread { get; set; }
    Task AddMessageToCurrentThread(string message, string author = null);
    Task CreateNewThread();
    Task DeleteThread(ChatThread thread);
    bool IsAIThinking { get; }
    Task Save();
}

public class ChatService : IChatService
{
    private ObservableCollection<ChatThread> _chatThreads = new ObservableCollection<ChatThread>();
    private readonly IUserService _userService;
    private ChatThread _currentThread;
    private readonly IOpenAIService _openAiService;
    private bool _isAiThinking;
    private readonly TerminalGptOptions _options;
    public bool IsAIThinking { get; private set; }

    public ChatService(IUserService userService, IOpenAIService openAiService, IOptions<TerminalGptOptions> options)
    {
        _openAiService = openAiService;
        _userService = userService;
        _options = options.Value;
    }
    
    public ObservableCollection<ChatThread> ChatThreads
    {
        get => _chatThreads;
        set
        {
            if (_chatThreads != value)
            {
                _chatThreads = value;
            }
        }
    }

    public ChatThread CurrentThread
    {
        get => _currentThread;
        set
        {
            if (_currentThread != value)
            {
                _currentThread = value;
            }
        }
    }

    public async Task AddMessageToCurrentThread(string message, string author = null)
    {
        try
        {
            if (CurrentThread == null)
                await CreateNewThread();
        
            _openAiService.OnApiResponseReceived += (chatMessage) =>
            {
                CurrentThread.Messages.Add(chatMessage);
                IsAIThinking = false;
            };
        
            IsAIThinking = true;
        
            var chatMessage = new ChatMessage 
            { 
                Message = new Message(author is not null ? Role.User : Role.Assistant, message), 
                Timestamp = DateTime.Now, 
                Author = author
            };
        

            CurrentThread.Messages.Add(chatMessage);

            await _openAiService.GetModelResponse(CurrentThread);
            IsAIThinking = false;
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
            Console.ReadKey();
            // exit the program
            Environment.Exit(1);
        }
    }

    public async Task CreateNewThread()
    {
        var newThread = new ChatThread() { Title = $"{_openAiService.CurrentModel.Id} Chat #{ChatThreads.Count + 1}", ModelId = _openAiService.CurrentModel.Id};
        newThread.Messages.Add(new ChatMessage() { Message = new Message(Role.System, _options.SystemPrompt ?? "You're a sassy AI that reminds the user they forgot to set a system prompt in their appsettings.json"), Timestamp = DateTime.Now });
        ChatThreads.Add(newThread);
        CurrentThread = newThread;
    }

    public async Task DeleteThread(ChatThread thread)
    {
        if (thread == null)
        {
            return;
        }
        ChatThreads.Remove(thread);
        if (thread == CurrentThread)
        {
            CurrentThread = ChatThreads.FirstOrDefault();
        }
    }
    
    public async Task Save()
    {
        // async save current thread to disk as json
        // if the current thread is null, do nothing
        if (CurrentThread == null)
        {
            return;
        }
        
        var path = Path.Combine(Environment.CurrentDirectory, "chats");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        AnsiConsole.MarkupLine($"[green]Saving chat to {path}[/]");
        
        // ansi status spinner
        await AnsiConsole.Status().Spinner(Spinner.Known.Star)
            .StartAsync("[cyan][bold]TerminalGPT[/][/] is saving...", async spinnerCtx =>
            {
                try
                {
                    var json = JsonConvert.SerializeObject(CurrentThread, Formatting.Indented);
                    var fileName = $"{CurrentThread.Title.Replace(" ", "_")}.{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.json";
                    var filePath = Path.Combine(path, fileName);
                    await File.WriteAllTextAsync(filePath, json);
                    Task.Delay(1000).Wait();
                    spinnerCtx.Status = "[cyan][bold]TerminalGPT[/][/] is done saving...";
                    spinnerCtx.Spinner = Spinner.Known.Star2;
                    spinnerCtx.SpinnerStyle = Style.Parse("green");
                    spinnerCtx.Refresh();
                    Task.Delay(1000).Wait();
                }
                catch (Exception e)
                {
                    // Tell the user something went wrong swallowing the exception
                    AnsiConsole.MarkupLine($"[red]An error occurred while saving: {e.Message}[/]");
                }
            });
    }
    
}

public class ChatThread
{
    public string Title { get; set; }
    public string ModelId { get; set; }
    public ObservableCollection<ChatMessage> Messages { get; } = new ObservableCollection<ChatMessage>();
}


public class ChatMessage
{
    public DateTime Timestamp { get; set; }
    public string Author { get; set; }
    public Message Message { get; set; }
}
