using System;
using Spectre.Console;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TerminalGPT.Constants;
using TerminalGPT.Options;
using Spectre.Console.Rendering;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using TerminalGPT.Data;
using TerminalGPT.Enums;
using TerminalGPT.Extensions;
using TerminalGPT.Constants;

namespace TerminalGPT.Services;
public interface ITerminalChatService : IBaseService
{
    Task Help();
    Task PrintCommandNotImplemented(CommandEnum.Command command);
}

public class TerminalChatService : BaseService, ITerminalChatService
{
    private readonly IChatService _chatService;
    private readonly IOpenAIService _openAiService;
    private List<IRenderable> items;
    private readonly IChatCommandService _commandService;
    private readonly IUserInputService _userInputService;
    private readonly ISettingsService _settingsService;

    public TerminalChatService(
        IChatService chatService,
        IOpenAIService openAiService,
        IChatCommandService commandService,
        IUserInputService userInputService,
        ISettingsService settingsService
    )
    {
        _chatService = chatService;
        _openAiService = openAiService;
        _commandService = commandService;
        _userInputService = userInputService;
        _settingsService = settingsService;
        items = new List<IRenderable>();
    }
    
    
    protected override async Task<ExitCode.Code> Run()
    {
        if (_chatService.CurrentThread == null)
            await _chatService.CreateNewThread();
        
        DrawHeader();

        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                AnsiConsole.Background = Color.DarkBlue;
                var input = string.Empty;

                // Hide the cursor
                Console.CursorVisible = false;
                while (string.IsNullOrWhiteSpace(input = await _userInputService.GetInputWithLiveDisplay()) && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(1);
                }

                // Show the cursor
                Console.CursorVisible = true;

                if (_commandService.TryParseCommand(input, out var command))
                {
                    await _commandService.ExecuteCommand(command);
                    if (command.CommandEnum != CommandEnum.Command.NotImplemented &&
                        command.CommandEnum != CommandEnum.Command.Unknown)
                        DrawCommand(command);

                    if (command.CommandEnum == CommandEnum.Command.Exit)
                    {
                        _cancellationTokenSource.Cancel();
                        return ExitCode.Code.CleanExit;
                    }
                }
                else
                {
                    AddItemToDisplay(input, $"{Environment.UserName} - {DateTime.Now}", true);

                    // get random number between 0 and 49
                    var random = new Random();
                    var randomIndex = random.Next(0, 49);


                    await AnsiConsole.Status().Spinner(Spinner.Known.Star)
                        .StartAsync(
                            $"[cyan][bold]TerminalGPT[/][/] is thinking... Fact: #{AppConstants.GPT_FACTS[randomIndex]}",
                            async spinnerCtx =>
                            {
                                await _chatService.AddMessageToCurrentThread(input, "User");

                                spinnerCtx.Status = "[cyan][bold]TerminalGPT[/][/] is done thinking...";
                                spinnerCtx.Spinner = Spinner.Known.Star2;
                                spinnerCtx.SpinnerStyle = Style.Parse("green");
                                spinnerCtx.Refresh();

                                Task.Delay(1000).Wait();
                            });

                    var response = _chatService.CurrentThread.Messages.Last().Message.Content;

                    AddItemToDisplay(response, $"TerminalGPT - {DateTime.Now}");
                }
            }

            return ExitCode.Code.Success;
        }
        catch (Exception e)
        {
            _cancellationTokenSource.Cancel();
            Console.WriteLine(e);
            return ExitCode.Code.Error;
        }
    }

    private void DrawCommand(ChatCommand command) =>
        AddItemToDisplay(command.CommandEnum.GetDescription(),
            $"Command::[gold3][bold]{command.CommandEnum.ToString()}[/][/] - {DateTime.Now}", true);

    public void DrawHeader()
    {
        var title =
            $"( ::[orange1][bold]Title: {_chatService.CurrentThread.Title} | Messages: {_chatService?.CurrentThread?.Messages?.Where(m => m.Message.Role != Role.System).Count() ?? 0}[/][/]:: )";

        try
        {
            AnsiConsole.Write(new FigletText(
                    FigletFont.Parse(AppConstants.FigletFont),
                    "T e r m i n a l . G P T")
                .Centered()
                .Color(Color.Aquamarine3)
            );

            if (_settingsService.Options.WhereArtThouTimmy)
            {
                if (File.Exists("RobotImg.png"))
                {
                    var image = new CanvasImage("RobotImg.png");
                    image.Resampler = new BicubicResampler();
                    image.PixelWidth = 3;
                    image.MaxWidth = 9;

                    AnsiConsole.Render(
                        new Table().Border(TableBorder.Rounded)
                            .AddColumn("Timmy The Terminal Robot")
                            .AddRow(image)
                            .Centered());
                }
            }

            if (_settingsService.Options.ShowAboutInfo)
                AnsiConsole.Render(
                    new Table().Border(TableBorder.Rounded)
                        .AddColumn("Version")
                        .AddColumn("GitHub Repo")
                        .AddRow("[red][bold]1.0.0[/][/]",
                            "[blue][link=https://github.com/LemonDrop1228/Terminal.GPT]https://github.com/LemonDrop1228/Terminal.GPT[/][/]")
                        .Centered());

            AnsiConsole.Render(_settingsService.Options.ShowChatTitle ? new Rule(title) : new Rule());

            if (_settingsService.Options.ShowSystemPrompt)
                AnsiConsole.Render(new Panel(new Markup(
                            $"[dim]{ExtractSystemPromptMessageContent()}[/]").Justify(Justify.Center)
                        .Centered())
                    .Header($"( ::[cornsilk1][bold]system prompt[/][/]:: )")
                    .HeaderAlignment(Justify.Center)
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.DarkGoldenrod)
                    .Expand()
                    .RoundedBorder()
                );

            string ExtractSystemPromptMessageContent()
            {
                return _chatService
                           ?.CurrentThread
                           ?.Messages
                           ?.FirstOrDefault(x => x.Message.Role == Role.System)
                           ?.Message
                           ?.Content
                       ?? _settingsService.Options.SystemPrompt ?? AppConstants.DefaultSystemPromptMessage
                    ;
            }
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            _cancellationTokenSource.Cancel();
        }
    }

    public void AddItemToDisplay(string content, string header, bool isUser = false)
    {
        items.Add(new Rule(
                isUser
                    ? $"[bold][green]{header}[/][/]"
                    : $"[bold][cyan]{header}[/][/]"
            )
            .RuleStyle(Style.Parse("grey"))
            .Justify(Justify.Left)
        );
            
        items.Add(new Panel(content).Border(BoxBorder.None));

        
        DrawItems();
    }

    public void DrawItems()
    {
        AnsiConsole.Clear();
        DrawHeader();
        AnsiConsole.WriteLine();

        foreach (var item in items)
        {
            AnsiConsole.Render(item);
            AnsiConsole.WriteLine();
        }
        
        AnsiConsole.Render(new Rule().RuleStyle(Style.Parse("gold3")));
    }

    public async Task Help()
    {
        throw new NotImplementedException();
    }

    public async Task PrintCommandNotImplemented(CommandEnum.Command command)
    {
        await AnsiConsole.Status().Spinner(Spinner.Known.Star)
            .StartAsync(
                command switch
                {
                    CommandEnum.Command.NotImplemented => "[red][bold] Command not yet implemented [/][/]",
                    _ => "[red][bold] Command not supported [/][/]"
                }, async spinnerCtx =>
                {
                    spinnerCtx.Spinner = Spinner.Known.Star2;
                    spinnerCtx.SpinnerStyle = Style.Parse("Yellow");
                    spinnerCtx.Refresh();
                    Task.Delay(1500).Wait();
                });
    }
}