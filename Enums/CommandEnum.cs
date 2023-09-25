using System.ComponentModel;

namespace TerminalGPT.Enums;

public class CommandEnum
{
    public Command Value { get; private set; }
    
    public void Set(Command value) => Value = value;

    public enum Command
    {
        [Description("Display this help message")]
        Help,
        [Description("Exit the application")]
        Exit,
        Clear,
        List,
        Menu,
        Switch,
        Create,
        Delete,
        Rename,
        Set,
        Get,
        [Description("Save the current chat session to a file")]
        Save,
        Load,
        History,
        Unknown,
        NotImplemented
    }
    
}