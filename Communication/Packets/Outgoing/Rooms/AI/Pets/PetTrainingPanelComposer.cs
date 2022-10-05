namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.Pets;

internal class PetTrainingPanelComposer : ServerPacket
{
    public PetTrainingPanelComposer(Pet petData)
        : base(ServerPacketHeader.PET_TRAINING_PANEL_MESSAGE_COMPOSER)
    {
        this.WriteInteger(petData.PetId);

        var AvailableCommands = new List<short>();

        this.WriteInteger(petData.PetCommands.Count);
        foreach (var Sh in petData.PetCommands.Keys)
        {
            this.WriteInteger(Sh);
            if (petData.PetCommands[Sh])
            {
                AvailableCommands.Add(Sh);
            }
        }

        this.WriteInteger(AvailableCommands.Count);
        foreach (var Sh in AvailableCommands)
        {
            this.WriteInteger(Sh);
        }
    }
}
