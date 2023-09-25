using Spectre.Console;

namespace TerminalGPT.Services;

public interface IUserInputService
{
    Task<string> GetInputWithLiveDisplay();
}

public class UserInputService : IUserInputService
{
    private const int _maxCursorFrames = 48;

    public async Task<string> GetInputWithLiveDisplay()
    {
        var prompt = $"( [green][bold]{Environment.UserName}'s input )[/][/]";
        var input = string.Empty;
        var renderable = GetPanel(header: prompt);
        return await AnsiConsole.Live(renderable)
            .AutoClear(true)
            .Overflow(VerticalOverflow.Visible)
            .StartAsync(async ctx =>
            {
                ctx.Refresh();

                // Use a cancellation token to communicate across tasks.
                var cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;

                // Start a task to read key input
                var keyTask = Task.Run(() => { return Console.ReadKey(true); }, token);

                var manualCursorFrames = _maxCursorFrames;
                var visibleFrames = 24;
                while (true)
                {
                    // If the key input task completed, process input.
                    if (keyTask.IsCompletedSuccessfully)
                    {
                        var key = keyTask.Result;

                        switch (key.Key)
                        {
                            case ConsoleKey.PageDown:
                                input += Environment.NewLine;
                                break;
                            case ConsoleKey.Enter:
                                return input;
                            case ConsoleKey.Backspace:
                                if (input.Length > 0)
                                {
                                    input = input.Remove(input.Length - 1);
                                }

                                break;
                            default:
                                input += key.KeyChar;
                                break;
                        }

                        // Restart the key reading task for the next loop
                        cancellationTokenSource = new CancellationTokenSource();
                        token = cancellationTokenSource.Token;
                        keyTask = Task.Run(() => Console.ReadKey(true), token);
                    }

                    ctx.UpdateTarget(GetPanel(prompt, input.Length > 0 ? input : null,
                        (manualCursorFrames <= visibleFrames ? true : false)));
                    ctx.Refresh();
                    await Task.Delay(1, token);
                    ctx.Refresh();

                    if (manualCursorFrames-- <= 0)
                        manualCursorFrames = _maxCursorFrames;
                }

                return input;
            });
    }


    private Panel GetPanel(string header, string input1 = null, bool showCursor = true)
    {
        var blinkingRedCaret = "[blink][bold][red]_[/][/][/]";
        var redCaret = (showCursor ? blinkingRedCaret : string.Empty);
        var markupText = " ";
        if (input1 is not null)
        {
            markupText = (input1.StartsWith("/") && !input1.Contains(" "))
                ? $"[yellow]/[/][purple]{input1[1..]}[/]"
                : $"{input1}{redCaret}";
        }
        else
            markupText = $" {redCaret}";

        return new Panel(new Markup(markupText))
            .Header(header)
            .Border(BoxBorder.Ascii)
            .HeaderAlignment(Justify.Center)
            .Expand();
    }
}