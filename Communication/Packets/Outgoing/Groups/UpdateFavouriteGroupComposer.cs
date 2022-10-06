namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Groups;

internal class UpdateFavouriteGroupComposer : ServerPacket
{
    public UpdateFavouriteGroupComposer(Group group, int virtualId)
        : base(ServerPacketHeader.FAVORITE_GROUP_UDPATE)
    {
        this.WriteInteger(virtualId);//Sends 0 on .COM
        this.WriteInteger(group != null ? group.Id : 0);
        this.WriteInteger(3);
        this.WriteString(group != null ? group.Name : string.Empty);
    }
}
