using TerminalGPT.Enums;
using TerminalGPT.Extensions;

namespace TerminalGPT.Data;

public class ChatCommand
{
    public CommandEnum.Command CommandEnum { get; }

    // get enum description
    public string Description => CommandEnum.GetDescription();

    public ChatCommand(CommandEnum.Command commandEnum)
    {
        CommandEnum = commandEnum;
    }
}