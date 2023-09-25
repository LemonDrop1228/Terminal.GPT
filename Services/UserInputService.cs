using Spectre.Console;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Spectre.Console.Rendering;

namespace TerminalGPT.Services;

public interface IUserInputService
{
    Task<string> GetInputWithLiveDisplay();
}

public class UserInputService : IUserInputService
{
    private const int _maxCursorFrames = 48;
    private int _cursorPosition = 0;

    public async Task<string> GetInputWithLiveDisplay()
    {
        _cursorPosition = 0;
        var prompt = $"( [green][bold]{Environment.UserName}'s input )[/][/]";
        var input = new List<char>();
        var renderable = GetPanel(header: prompt);

        return await AnsiConsole.Live(renderable)
            .AutoClear(true)
            .Overflow(VerticalOverflow.Visible)
            .StartAsync(async ctx =>
            {
                try
                {
                    ctx.Refresh();

                    var cancellationTokenSource = new CancellationTokenSource();
                    var token = cancellationTokenSource.Token;
                    var keyTask = Task.Run(() => { return Console.ReadKey(true); }, token);
                    var manualCursorFrames = _maxCursorFrames;
                    var visibleFrames = 24;

                    while (true)
                    {
                        if (keyTask.IsCompletedSuccessfully)
                        {
                            var key = keyTask.Result;

                            switch (key.Key)
                            {
                                case ConsoleKey.PageDown:
                                    input.Insert(_cursorPosition, '\n');
                                    _cursorPosition++;
                                    break;
                                case ConsoleKey.Enter:
                                    cancellationTokenSource.Cancel();
                                    return new string(input.ToArray());
                                case ConsoleKey.Backspace:
                                    if (_cursorPosition > 0)
                                    {
                                        _cursorPosition--;
                                        input.RemoveAt(_cursorPosition);
                                    }

                                    break;
                                case ConsoleKey.Delete:
                                    if (_cursorPosition < input.Count)
                                    {
                                        input.RemoveAt(_cursorPosition);
                                    }

                                    break;
                                case ConsoleKey.Escape:
                                    input.InsertRange(_cursorPosition, "/Exit".ToCharArray());
                                    _cursorPosition += 5;
                                    break;
                                case ConsoleKey.Tab:
                                    input.InsertRange(_cursorPosition, "    ".ToCharArray());
                                    _cursorPosition += 4;
                                    break;
                                case ConsoleKey.Home:
                                    _cursorPosition = 0;
                                    break;
                                case ConsoleKey.End:
                                    _cursorPosition = input.Count;
                                    break;
                                case ConsoleKey.LeftArrow:
                                    if (key.Modifiers == ConsoleModifiers.Control)
                                        _cursorPosition = MoveBackToWordBoundary(input, _cursorPosition);
                                    else
                                        _cursorPosition = Math.Max(0, _cursorPosition - 1);
                                    break;
                                case ConsoleKey.RightArrow:
                                    if (key.Modifiers == ConsoleModifiers.Control)
                                        _cursorPosition = MoveForwardToWordBoundary(input, _cursorPosition);
                                    else
                                        _cursorPosition = Math.Min(input.Count, _cursorPosition + 1);
                                    break;
                                default:
                                    if (!char.IsControl(key.KeyChar))
                                    {
                                        input.Insert(_cursorPosition, key.KeyChar);
                                        _cursorPosition++;
                                    }

                                    break;
                            }


                            cancellationTokenSource = new CancellationTokenSource();
                            token = cancellationTokenSource.Token;
                            keyTask = Task.Run(() => Console.ReadKey(true), token);
                        }

                        ctx.UpdateTarget(GetPanel(prompt, input.Count > 0 ? new string(input.ToArray()) : null,
                            _cursorPosition,
                            (manualCursorFrames <= visibleFrames ? true : false)));
                        ctx.Refresh();
                        await Task.Delay(1, token);
                        ctx.Refresh();

                        if (manualCursorFrames-- <= 0)
                            manualCursorFrames = _maxCursorFrames;
                    }
                }
                catch (Exception e)
                {
                    AnsiConsole.WriteException(e);
                    throw;
                }
            });
    }

    private int MoveBackToWordBoundary(List<char> input, int currentPosition)
    {
        for (int i = currentPosition - 1; i > 0; i--)
            if (input[i] == ' ' && input[i - 1] != ' ')
                return i;

        return 0;
    }

    private int MoveForwardToWordBoundary(List<char> input, int currentPosition)
    {
        for (int i = currentPosition; i < input.Count - 1; i++)
            if (input[i] != ' ' && input[i + 1] == ' ')
                return i + 1;

        return input.Count;
    }


    private Panel GetPanel(string header, string input1 = null,
        int cursorPosition = 0, bool showCursor = true)
    {
        var caret = (showCursor ? "[bold][red]_[/][/]" : "[grey]_[/]");
        var markupText = " ";
        bool isCommand = false;

        if (input1 is not null)
        {
            var markupInput = input1.Substring(0, cursorPosition) +
                              input1.Substring(cursorPosition);
            if (markupInput.StartsWith("/"))
            {
                markupText = $"[yellow]/[/][purple]{markupInput[1..]}[/]";
                isCommand = true;
            }
            else
            {
                markupText = markupInput;
            }
        }
        else
            markupText = $" ";

        return new Panel(new Markup(isCommand ? $"{markupText}{caret}" : $"{Markup.Escape(markupText)}{caret}"))
            .Header(header)
            .Border(BoxBorder.Ascii)
            .HeaderAlignment(Justify.Center)
            .Expand();
    }

    private string ScrubMarkup(string content)
    {
        var regex = new Regex(@"\[(.*?)\]");
        content = regex.Replace(content, "");
        return content;
    }
}