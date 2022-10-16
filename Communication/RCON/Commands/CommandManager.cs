namespace WibboEmulator.Communication.RCON.Commands;
using WibboEmulator.Communication.RCON.Commands.Hotel;
using WibboEmulator.Communication.RCON.Commands.User;

public class CommandManager
{
    private readonly Dictionary<string, IRCONCommand> _commands;

    public CommandManager()
    {
        this._commands = new Dictionary<string, IRCONCommand>();

        this.RegisterUser();
        this.RegisterHotel();
    }

    public bool Parse(string data)
    {
        if (data.Length == 0 || string.IsNullOrEmpty(data))
        {
            return false;
        }

        var cmd = data.Split(Convert.ToChar(1))[0];

        if (this._commands.TryGetValue(cmd.ToLower(), out var command))
        {
            string[] parameters = data.Split(Convert.ToChar(1));

            return command.TryExecute(parameters);
        }
        return false;
    }

    private void RegisterUser()
    {
        this.Register("addphoto", new AddPhotoCommand());
        this.Register("addwinwin", new AddWinwinCommand());
        this.Register("updatecredits", new UpdateCreditsCommand());
        this.Register("updatepoints", new UpdateWibboPointsCommand());
        this.Register("updateltc", new UpdateLimitCoinsCommand());
        this.Register("signout", new SignOutCommand());
        this.Register("ha", new HaCommand());
        this.Register("eventha", new EventHaCommand());
        this.Register("useralert", new UserAlertCommand());
        this.Register("senduser", new SendUserCommand());
        this.Register("follow", new FollowCommand());
    }

    private void RegisterHotel()
    {
        this.Register("unload", new UnloadCommand());
        this.Register("updatenavigator", new UpdateNavigatorCommand());
        this.Register("shutdown", new ShutdownCommand());
        this.Register("autogame", new AutoGameCommand());
    }

    public void Register(string commandText, IRCONCommand command) => this._commands.Add(commandText, command);
}
