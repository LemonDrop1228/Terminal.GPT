namespace TerminalGPT.Enums;

public class ServiceMode
{
    public static Mode Value { get; private set; } = Mode.Chat;
    
    public static void Set(Mode mode)
    {
        Value = mode;
    }
    
    public enum Mode
    {
        Chat,
        Menu,
        Help,
        Exit
    }
}