using System.Collections.ObjectModel;
using System.Diagnostics;
using OpenAI;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OpenAI.Models;
using TerminalGPT.Extensions;
using static TerminalGPT.Constants.AppConstants;
using OpenAI.Completions;
using TerminalGPT.Options;

namespace TerminalGPT.Services;

public interface IOpenAIService
{
    bool IsApiKeyValid { get; }
    ObservableCollection<Model> Models { get; }
    Action<ChatMessage> OnApiResponseReceived { get; set; }
    Model CurrentModel { get; }
    Task GetModelResponse(ChatThread thread);
    Task<bool> ValidateApiKeyAsync();
}

public class OpenAiService : IOpenAIService
{
    private readonly IOpenAIClientFactory _openAIClientFactory;
    private ObservableCollection<Model> _models;
    public ObservableCollection<Model> Models { get; private set; }
    public Model CurrentModel => Models.FirstOrDefault(model => model.Id == _options.Model.GetId());
    public Action<ChatMessage> OnApiResponseReceived { get; set; }
    
    private bool _isApiKeyValid;
    private readonly TerminalGptOptions _options;

    public bool IsApiKeyValid {get; private set;}

    public OpenAiService(IOpenAIClientFactory openAIClientFactory, IOptions<TerminalGptOptions> options)
    {
        _openAIClientFactory = openAIClientFactory;
        _options = options.Value;
    }
    
    // TODO: refactor this to yield return the response message instead of using an event that way the response can be streamed to the console as it comes in
    public async Task GetModelResponse(ChatThread thread)
    {
        var messages = thread.Messages.Select(message => message.Message).ToList();

        ChatMessage responseMessage = null;
        
        var chatRequest = new ChatRequest(messages, thread.ModelId, number: 1);
        await _openAIClientFactory.Client.ChatEndpoint.StreamCompletionAsync(chatRequest, result =>
        {
            var response = result.Choices.FirstOrDefault(choice => !string.IsNullOrWhiteSpace(choice.Message?.Content));
            if (response != null)
            {
                Debug.WriteLine($"{response.Message.Role}: {response.Message.Content}");
                responseMessage = new ChatMessage()
                {
                    Message = response.Message,
                    Author = "AI",
                    Timestamp = DateTime.Now
                };
            }
            
        });
        
        OnApiResponseReceived?.Invoke(responseMessage);
        
        // clear OnApiResponseReceived so that it doesn't get called again
        OnApiResponseReceived = null;
    }
    
    public async Task<bool> ValidateApiKeyAsync()
    {
        try
        {
            var results = await _openAIClientFactory.Client.ModelsEndpoint.GetModelsAsync();
            Models = new ObservableCollection<Model>(results);
            return IsApiKeyValid = true;
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex);
            return IsApiKeyValid = false;
        }
    }
}