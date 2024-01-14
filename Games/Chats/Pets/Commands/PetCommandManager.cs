namespace WibboEmulator.Games.Chats.Pets.Commands;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public class PetCommandManager
{
    private readonly Dictionary<string, PetCommand> _petCommands;

    public PetCommandManager() => this._petCommands = new Dictionary<string, PetCommand>();

    public void Init(IDbConnection dbClient)
    {
        this._petCommands.Clear();

        var emulatorCommandList = EmulatorCommandPetDao.GetAll(dbClient);

        if (emulatorCommandList.Count == 0)
        {
            return;
        }

        foreach (var emulatorCommand in emulatorCommandList)
        {
            var key = emulatorCommand.Id;
            var command = emulatorCommand.Command;

            this._petCommands.Add(command, new PetCommand(key, command));
        }
    }

    public int TryInvoke(string input)
    {
        if (this._petCommands.TryGetValue(input, out var petCommand))
        {
            return petCommand.CommandID;
        }
        else
        {
            return 99;
        }
    }
}
