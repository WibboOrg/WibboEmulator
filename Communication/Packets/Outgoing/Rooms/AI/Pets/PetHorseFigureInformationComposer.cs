using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class PetHorseFigureInformationComposer : ServerPacket
    {
        public PetHorseFigureInformationComposer(RoomUser PetUser)
            : base(ServerPacketHeader.PET_FIGURE_UPDATE)
        {
            this.WriteInteger(PetUser.PetData.VirtualId);
            this.WriteInteger(PetUser.PetData.PetId);
            this.WriteInteger(PetUser.PetData.Type);
            this.WriteInteger(int.Parse(PetUser.PetData.Race));
            this.WriteString(PetUser.PetData.Color.ToLower());
            this.WriteInteger(1);
            if (PetUser.PetData.Saddle > 0)
            {
                this.WriteInteger(3); //Count

                this.WriteInteger(2);
                this.WriteInteger(PetUser.PetData.PetHair);
                this.WriteInteger(PetUser.PetData.HairDye);

                this.WriteInteger(3);
                this.WriteInteger(PetUser.PetData.PetHair);
                this.WriteInteger(PetUser.PetData.HairDye);

                this.WriteInteger(4);
                this.WriteInteger(PetUser.PetData.Saddle);
                this.WriteInteger(0);
            }
            else
            {

                this.WriteInteger(2); //Count

                this.WriteInteger(2);
                this.WriteInteger(PetUser.PetData.PetHair);
                this.WriteInteger(PetUser.PetData.HairDye);

                this.WriteInteger(3);
                this.WriteInteger(PetUser.PetData.PetHair);
                this.WriteInteger(PetUser.PetData.HairDye);
            }
            this.WriteBoolean(PetUser.PetData.Saddle > 0);
            this.WriteBoolean(PetUser.RidingHorse);
        }
    }
}
