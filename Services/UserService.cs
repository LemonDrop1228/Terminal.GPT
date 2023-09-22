using System;

namespace TerminalGPT.Services;

public interface IUserService
{
    string GetCurrentUserName();
}

public class UserService : IUserService
{
    public string GetCurrentUserName()
    {
        return Environment.UserName;
    }
}