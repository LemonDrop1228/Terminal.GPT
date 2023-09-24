using Spectre.Console;
using Spectre.Console.Rendering;
using TerminalGPT.Constants;
using TerminalGPT.Enums;
using System.Threading.Tasks;

namespace TerminalGPT.Services;

public interface IMenuService : IBaseService
{
}

public class MenuService : BaseService, IMenuService
{
    protected override async Task<ExitCode.Code> Run()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            // Show the header, info table, and divider like in the chat service.
            DrawHeader();
            
            // Show the menu options
            DrawMenu();
            
            await Task.Delay(1000, _cancellationTokenSource.Token);
        }
        
        // do clean up here
        
        
        // return the exit code
        return ExitCode.Code.Success;
    }

    private void DrawMenu()
    {
        var panel = new Panel(CreateMenuTable())
            .Header("[gold3][bold]Menu[/][/]")
            .BorderColor(Color.Gold3)
            .Expand();
        
        IRenderable CreateMenuTable()
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumns("Chat Menu", "Command Menu", "Settings Menu", "Help Menu", "Exit Menu");
            table.AddRow("[cyan]1.[/] Chat Menu", "[cyan]2.[/] Command Menu", "[cyan]3.[/] Settings Menu", "[cyan]4.[/] Help Menu", "[cyan]5.[/] Exit Menu");
            return table;
        }
    }
    
    private static Table AddColumns(Table table, params string[] columns)
    {
        foreach (var column in columns)
        {
            table.AddColumn(column);
        }

        return table;
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
}
