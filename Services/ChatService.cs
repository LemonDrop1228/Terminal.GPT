using System.Collections.ObjectModel;
using System.Collections.Specialized;
using OpenAI.Chat;
using OpenAI.Models;
using Spectre.Console;

namespace TerminalGPT.Services;

public interface IChatService
{
    ObservableCollection<ChatThread> ChatThreads { get; set; }
    ChatThread? CurrentThread { get; set; }
    Task AddMessageToCurrentThread(string message, string author = null);
    Task CreateNewThread();
    Task DeleteThread(ChatThread thread);
    bool IsAIThinking { get; }
}

public class ChatService : IChatService
{
    private ObservableCollection<ChatThread> _chatThreads = new ObservableCollection<ChatThread>();
    private readonly IUserService _userService;
    private ChatThread _currentThread;
    private readonly IOpenAIService _openAiService;
    private bool _isAiThinking;
    public bool IsAIThinking { get; private set; }

    public ChatService(IUserService userService, IOpenAIService openAiService)
    {
        _openAiService = openAiService;
        _userService = userService;
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
        newThread.Messages.Add(new ChatMessage() { Message = new Message(Role.System, "You're a Wiki AI used for learning."), Timestamp = DateTime.Now });
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
