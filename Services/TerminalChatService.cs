using System;
using Spectre.Console;
using System.Linq;
using System.Collections.Generic;
using TerminalGPT.Constants;
using TerminalGPT.Options;
using Spectre.Console.Rendering;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace TerminalGPT.Services
{
    public interface ITerminalChatService
    {
        Task RunChat();
        void DrawHeader();
        void AddItemToDisplay(string content, string header, bool isUser = false);
        void DrawItems();
    }

    public class TerminalChatService : ITerminalChatService
    {
        private readonly IChatService _chatService;
        private readonly IOpenAIService _openAiService;
        private readonly TerminalGptOptions _options;
        private List<IRenderable> items;

        public TerminalChatService(IChatService chatService, IOpenAIService openAiService,
            IOptions<TerminalGptOptions> options)
        {
            _chatService = chatService;
            _openAiService = openAiService;
            _options = options.Value;
            items = new List<IRenderable>();
        }

        public async Task RunChat()
        {
            DrawHeader();

            try
            {
                while (true)
                {
                    AnsiConsole.Background = Color.DarkBlue;
                    var input = AnsiConsole.Ask<string>($"[cyan][bold]{Environment.UserName}[/][/]:");

                    AddItemToDisplay(input, $"[cyan][bold]{Environment.UserName}[/][/] - {DateTime.Now}", true);

                    await AnsiConsole.Status().Spinner(Spinner.Known.Star)
                        .StartAsync("[gold3][bold]TerminalGPT[/][/] is thinking...", async spinnerCtx =>
                        {
                            await _chatService.AddMessageToCurrentThread(input, "User");
                            
                            spinnerCtx.Status = "[gold3][bold]TerminalGPT[/][/] is done thinking...";
                            spinnerCtx.Spinner = Spinner.Known.Star2;
                            spinnerCtx.SpinnerStyle = Style.Parse("green");
                            spinnerCtx.Refresh();
                            
                            Task.Delay(1000).Wait();
                        });

                    var response = _chatService.CurrentThread.Messages.Last().Message.Content;

                    AddItemToDisplay(response, $"TerminalGPT - {DateTime.Now}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }

        public void DrawHeader()
        {
            AnsiConsole.Write(new FigletText(
                    FigletFont.Parse(AppConstants.FigletFont),
                    "T e r m i n a l . G P T")
                .Centered()
                .Color(Color.Aquamarine3)
            );
            
            AnsiConsole.Render(
                new Table().Border(TableBorder.Rounded)
                    .AddColumn("App")
                    .AddColumn("Version")
                    .AddColumn("GitHub Repo")
                    .AddRow("[gold1]T[/]erminal[gold1]GPT[/]", "[red][bold]1.0.0[/][/]",
                        "[blue][link=https://github.com/LemonDrop1228/Terminal-GPT]https://github.com/LemonDrop1228/Terminal-GPT[/][/]")
                    .Centered());
            AnsiConsole.Render(new Rule());
        }

        public void AddItemToDisplay(string content, string header, bool isUser = false)
        {
            items.Add(new Panel($"{content}").Header(isUser
                ? $"[cyan][bold]{header}[/][/]"
                : $"[gold3][bold]{header}[/][/]"));
            items.Add(new Rule().RuleStyle(Style.Parse("cyan")));
            DrawItems();
        }

        public void DrawItems()
        {
            AnsiConsole.Clear();
            DrawHeader();

            foreach (var item in items)
            {
                AnsiConsole.Render(item);
            }
        }
    }
}
