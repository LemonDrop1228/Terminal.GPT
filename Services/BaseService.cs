using TerminalGPT.Enums;

namespace TerminalGPT.Services;


public interface IBaseService
{
    Task Stop();
    Task<ExitCode.Code> Start();
}

public abstract class BaseService : IBaseService
{
    protected CancellationTokenSource _cancellationTokenSource;
    private Task _task;

    public async Task Stop()
    {
        _cancellationTokenSource.Cancel();
        await _task;
    }

    public async Task<ExitCode.Code> Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        return await Run();
    }

    protected abstract Task<ExitCode.Code> Run();
}