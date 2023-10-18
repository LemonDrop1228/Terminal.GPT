using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using TerminalGPT.Options;
using Microsoft.Extensions.Options;
using TerminalGPT.Enums;

namespace TerminalGPT.Services
{
    public class MainService
    {
        private readonly ITerminalChatService _terminalChatService;
        private readonly IOpenAIService _openAiService;
        private readonly IMenuService _menuService;
        private readonly IExitService _exitService;
        private readonly IChatCommandService _chatCommandService;

        public MainService(
            ITerminalChatService terminalChatService,
            IOpenAIService openAiService,
            IMenuService menuService,
            IExitService exitService,
            IChatCommandService chatCommandService
        )
        {
            _terminalChatService = terminalChatService;
            _openAiService = openAiService;
            _menuService = menuService;
            _exitService = exitService;
            _chatCommandService = chatCommandService;
        }

        public async Task Run()
        {
            _chatCommandService.InitializeCommands();
            try
            {
                while (ServiceMode.Value != ServiceMode.Mode.Exit)
                {
                    ExitCode.Code main = ServiceMode.Value switch
                    {
                        ServiceMode.Mode.Chat => await _terminalChatService.Start(),
                        ServiceMode.Mode.Menu => await _menuService.Start(),
                        ServiceMode.Mode.Exit => await _exitService.Exit(),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (main == ExitCode.Code.CleanExit)
                    {
                        ServiceMode.Set(ServiceMode.Mode.Exit);
                    }
                    else if (main == ExitCode.Code.Error)
                    {
                        Console.WriteLine("An error occurred. Exiting...");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadLine();
                        ServiceMode.Set(ServiceMode.Mode.Exit);
                    }

                    if (ServiceMode.Value == ServiceMode.Mode.Exit)
                        break;

                    ServiceMode.Set
                    (
                        ServiceMode.Value switch
                        {
                            ServiceMode.Mode.Chat => ServiceMode.Mode.Menu,
                            ServiceMode.Mode.Menu => ServiceMode.Mode.Chat
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ExitCode.Set(ExitCode.Code.Error);
            }
        }
    }
}