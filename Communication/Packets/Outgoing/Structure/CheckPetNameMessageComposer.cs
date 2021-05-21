using Butterfly.HabboHotel.Catalog.Utilities;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class CheckPetNameMessageComposer : ServerPacket
    {
        public CheckPetNameMessageComposer(string PetName)
            : base(ServerPacketHeader.CheckPetNameMessageComposer)
        {
            this.WriteInteger(PetUtility.CheckPetName(PetName) ? 0 : 2);
            this.WriteString(PetName);
        }
    }
}
