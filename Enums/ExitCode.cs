namespace TerminalGPT.Enums;

public class ExitCode
{
    public static Code Value { get; private set; } = Code.Success;
    
    public static void Set(Code code) => Value = code;

    public enum Code
    {
        Success,
        CleanExit,
        Error
    }
}