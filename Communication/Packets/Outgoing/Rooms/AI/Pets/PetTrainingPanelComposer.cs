namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.Rooms.AI;

internal sealed class PetTrainingPanelComposer : ServerPacket
{
    public PetTrainingPanelComposer(Pet petData)
        : base(ServerPacketHeader.PET_TRAINING_PANEL_MESSAGE_COMPOSER)
    {
        this.WriteInteger(petData.PetId);

        var availableCommands = new List<short>();

        this.WriteInteger(petData.PetCommands.Count);
        foreach (var sh in petData.PetCommands.Keys)
        {
            this.WriteInteger(sh);
            if (petData.PetCommands[sh])
            {
                availableCommands.Add(sh);
            }
        }

        this.WriteInteger(availableCommands.Count);
        foreach (var sh in availableCommands)
        {
            this.WriteInteger(sh);
        }
    }
}
