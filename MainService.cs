using Spectre.Console;
using System.Linq;
using Microsoft.Extensions.Options;
using Spectre.Console.Rendering;
using TerminalGPT.Constants;
using TerminalGPT.Extensions;
using TerminalGPT.Options;

namespace TerminalGPT.Services;

public class MainService
{
    private readonly IChatService _chatService;
    private IRenderable[] items;
    private readonly IOpenAIService _openAiService;
    private readonly TerminalGptOptions _options;

    public MainService(
        IChatService chatService,
        IOpenAIService openAiService,
        IOptions<TerminalGptOptions> options
    )
    {
        _chatService = chatService;
        _openAiService = openAiService;
        _options = options.Value;

        items = new IRenderable[] { };
    }

    public async Task Run()
    {
        AnsiConsole.Write(new FigletText(
                FigletFont.Parse(AppConstants.FigletFont),
                "Terminal-GPT")
            .Centered()
        );
        AnsiConsole.Render(
            new Table().Border(TableBorder.Rounded)
                .AddColumn("App")
                .AddColumn("Version")
                .AddColumn("GitHub Repo")
                .AddRow("TerminalGPT", "1.0.0", "[link=https://github.com/LemonDrop1228/Terminal-GPT]https://github.com/LemonDrop1228/Terminal-GPT[/]")
                .Centered());
        AnsiConsole.Render(new Rule());

        try
        {
            while (true)
            {
                // Ask for user input
                var input = AnsiConsole.Ask<string>("Prompt: ").Trim();

                // Add user input to the display
                var userHeader = $"{Environment.UserName} - {DateTime.Now}";
                items = items.Concat(new[]
                        {new Panel($"{input}").Header(userHeader) as IRenderable})
                    .ToArray();
                DrawItems();


                // Show a spinner while the AI is thinking
                await AnsiConsole.Status().Spinner(Spinner.Known.Star)
                    .StartAsync("[cyan][bold]TerminalGPT[/][/] is thinking...", async spinnerCtx =>
                    {
                        await _chatService.AddMessageToCurrentThread(input, "User");
                        
                        // AI is done
                        spinnerCtx.Status = "TerminalGPT is done thinking...";
                        spinnerCtx.Spinner = Spinner.Known.Star2;
                        spinnerCtx.SpinnerStyle = Style.Parse("green");
                        spinnerCtx.Refresh();
                        
                        Task.Delay(1000).Wait();
                    });

                var response = _chatService.CurrentThread.Messages.Last().Message.Content;

                items = items.Concat(new[] {new Rule().RuleStyle(Style.Parse("cyan")) as IRenderable})
                    .ToArray(); // divider line

                // Add AI's output to the display
                var assistantHeader = $"TerminalGPT - {DateTime.Now}";
                items = items.Concat(new[]
                    {new Panel($"{response}").Header($"{assistantHeader}") as IRenderable}).ToArray();


                items = items.Concat(new[] {new Rule().RuleStyle(Style.Parse("cyan")) as IRenderable})
                    .ToArray(); // divider line
                DrawItems();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.ReadKey();
        }
    }

    private void DrawItems()
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(new FigletText(
                FigletFont.Parse(AppConstants.FigletFont),
                "Terminal-GPT")
            .Centered()
        );
        AnsiConsole.Render(
            new Table().Border(TableBorder.Rounded)
                .AddColumn("App")
                .AddColumn("Version")
                .AddColumn("GitHub Repo")
                .AddRow("TerminalGPT", "1.0.0", "[link=https://github.com/your-repo]https://github.com/your-repo[/]")
                .Centered());
        AnsiConsole.Render(new Rule());

        foreach (var item in items)
        {
            AnsiConsole.Render(item);
        }
    }

    public async Task Initialize()
    {
        await _openAiService.ValidateApiKeyAsync();
    }
}