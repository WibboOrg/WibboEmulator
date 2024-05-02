namespace WibboEmulator.Games.Chats.Pets.Commands;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class PetCommandManager
{
    private static readonly Dictionary<string, PetCommand> PetCommands = new();

    public static void Initialize(IDbConnection dbClient)
    {
        PetCommands.Clear();

        var emulatorCommandList = EmulatorCommandPetDao.GetAll(dbClient);

        foreach (var emulatorCommand in emulatorCommandList)
        {
            var key = emulatorCommand.Id;
            var command = emulatorCommand.Command;

            PetCommands.Add(command, new PetCommand(key, command));
        }
    }

    public static int TryInvoke(string input)
    {
        if (PetCommands.TryGetValue(input, out var petCommand))
        {
            return petCommand.CommandID;
        }
        else
        {
            return 99;
        }
    }
}
