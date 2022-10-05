namespace WibboEmulator.Games.Chat.Pets.Commands;

public struct PetCommand
{
    public PetCommand(int CommandID, string CommandInput)
    {
        this.CommandID = CommandID;
        this.CommandInput = CommandInput;
    }

    public int CommandID { get; set; }

    public string CommandInput { get; set; }
}
