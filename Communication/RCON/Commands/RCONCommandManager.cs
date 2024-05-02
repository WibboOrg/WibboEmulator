namespace WibboEmulator.Communication.RCON.Commands;
using WibboEmulator.Communication.RCON.Commands.Hotel;
using WibboEmulator.Communication.RCON.Commands.User;

public static class RCONCommandManager
{
    private static readonly Dictionary<string, IRCONCommand> Commands = new();

    public static void Initialize()
    {
        RegisterUser();
        RegisterHotel();
    }

    public static bool Parse(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return false;
        }

        var cmd = data.Split(Convert.ToChar(1))[0];

        if (Commands.TryGetValue(cmd.ToLower(), out var command))
        {
            var parameters = data.Split(Convert.ToChar(1));

            return command.TryExecute(parameters);
        }

        return false;
    }

    private static void RegisterUser()
    {
        Register("addphoto", new AddPhotoCommand());
        Register("addwinwin", new AddWinwinCommand());
        Register("updatecredits", new UpdateCreditsCommand());
        Register("updatepoints", new UpdateWibboPointsCommand());
        Register("updateltc", new UpdateLimitCoinsCommand());
        Register("signout", new SignOutCommand());
        Register("ha", new HaCommand());
        Register("eventha", new EventHaCommand());
        Register("useralert", new UserAlertCommand());
        Register("senduser", new SendUserCommand());
        Register("follow", new FollowCommand());
    }

    private static void RegisterHotel()
    {
        Register("unload", new UnloadCommand());
        Register("updatenavigator", new UpdateNavigatorCommand());
        Register("shutdown", new ShutdownCommand());
        Register("autogame", new AutoGameCommand());
    }

    public static void Register(string commandText, IRCONCommand command) => Commands.Add(commandText, command);
}
