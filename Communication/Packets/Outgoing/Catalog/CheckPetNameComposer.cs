namespace WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.Catalogs.Utilities;

internal class CheckPetNameComposer : ServerPacket
{
    public CheckPetNameComposer(string petName)
        : base(ServerPacketHeader.CATALOG_APPROVE_NAME_RESULT)
    {
        this.WriteInteger(PetUtility.CheckPetName(petName) ? 0 : 2);
        this.WriteString(petName);
    }
}
