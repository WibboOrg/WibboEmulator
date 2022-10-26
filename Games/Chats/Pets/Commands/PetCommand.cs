namespace WibboEmulator.Games.Chat.Pets.Commands;

public struct PetCommand
{
    public PetCommand(int commandID, string commandInput)
    {
        this.CommandID = commandID;
        this.CommandInput = commandInput;
    }

    public int CommandID { get; set; }

    public string CommandInput { get; set; }
}
