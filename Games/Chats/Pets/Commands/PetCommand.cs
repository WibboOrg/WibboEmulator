namespace WibboEmulator.Games.Chats.Pets.Commands;

public struct PetCommand(int commandID, string commandInput)
{
    public int CommandID { get; set; } = commandID;

    public string CommandInput { get; set; } = commandInput;
}
