using Wibbo.Game.Catalog.Utilities;

namespace Wibbo.Communication.Packets.Outgoing.Catalog
{
    internal class CheckPetNameComposer : ServerPacket
    {
        public CheckPetNameComposer(string PetName)
            : base(ServerPacketHeader.CATALOG_APPROVE_NAME_RESULT)
        {
            this.WriteInteger(PetUtility.CheckPetName(PetName) ? 0 : 2);
            this.WriteString(PetName);
        }
    }
}
