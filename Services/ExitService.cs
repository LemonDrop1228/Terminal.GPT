using Spectre.Console;
using TerminalGPT.Enums;

namespace TerminalGPT.Services;

public interface IExitService
{
    Task<ExitCode.Code> Exit();
}

public class ExitService : IExitService
{
    private readonly IEnumerable<IBaseService> _services;

    public ExitService(IEnumerable<IBaseService> services)
    {
        _services = services;
    }
    
    public async Task<ExitCode.Code> Exit()
    {
        AnsiConsole.MarkupLine("[bold]Exiting TerminalGPT...[/]");
        foreach (var service in _services)
        {
            await service.Stop();
        }
        return ExitCode.Code.Success;
    }
}