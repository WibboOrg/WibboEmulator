namespace WibboEmulator.Games.Chat.Pets.Commands;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

public class PetCommandManager
{
    private readonly Dictionary<string, PetCommand> _petCommands;

    public PetCommandManager() => this._petCommands = new Dictionary<string, PetCommand>();

    public void Init(IQueryAdapter dbClient)
    {
        this._petCommands.Clear();

        var table = EmulatorCommandPetDao.GetAll(dbClient);

        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            var key = Convert.ToInt32(dataRow["id"]);
            var str1 = (string)dataRow["command"];

            this._petCommands.Add(str1, new PetCommand(key, str1));
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
