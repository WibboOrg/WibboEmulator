namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.Rooms;

internal class PetHorseFigureInformationComposer : ServerPacket
{
    public PetHorseFigureInformationComposer(RoomUser petUser)
        : base(ServerPacketHeader.PET_FIGURE_UPDATE)
    {
        this.WriteInteger(petUser.PetData.VirtualId);
        this.WriteInteger(petUser.PetData.PetId);
        this.WriteInteger(petUser.PetData.Type);
        this.WriteInteger(int.Parse(petUser.PetData.Race));
        this.WriteString(petUser.PetData.Color.ToLower());
        this.WriteInteger(1);
        if (petUser.PetData.Saddle > 0)
        {
            this.WriteInteger(3); //Count

            this.WriteInteger(2);
            this.WriteInteger(petUser.PetData.PetHair);
            this.WriteInteger(petUser.PetData.HairDye);

            this.WriteInteger(3);
            this.WriteInteger(petUser.PetData.PetHair);
            this.WriteInteger(petUser.PetData.HairDye);

            this.WriteInteger(4);
            this.WriteInteger(petUser.PetData.Saddle);
            this.WriteInteger(0);
        }
        else
        {

            this.WriteInteger(2); //Count

            this.WriteInteger(2);
            this.WriteInteger(petUser.PetData.PetHair);
            this.WriteInteger(petUser.PetData.HairDye);

            this.WriteInteger(3);
            this.WriteInteger(petUser.PetData.PetHair);
            this.WriteInteger(petUser.PetData.HairDye);
        }
        this.WriteBoolean(petUser.PetData.Saddle > 0);
        this.WriteBoolean(petUser.RidingHorse);
    }
}
