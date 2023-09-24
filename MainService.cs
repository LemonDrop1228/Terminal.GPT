using System;
using System.Threading.Tasks;
using Spectre.Console;
using TerminalGPT.Options;
using Microsoft.Extensions.Options;

namespace TerminalGPT.Services
{
    public class MainService
    {
        private readonly ITerminalChatService _terminalChatService;
        private readonly IOpenAIService _openAiService;

        public MainService(ITerminalChatService terminalChatService, IOpenAIService openAiService)
        {
            _terminalChatService = terminalChatService;
            _openAiService = openAiService;
        }

        public async Task Run()
        {
            try
            {
                await _openAiService.ValidateApiKeyAsync();
                await _terminalChatService.RunChat();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
    }
}