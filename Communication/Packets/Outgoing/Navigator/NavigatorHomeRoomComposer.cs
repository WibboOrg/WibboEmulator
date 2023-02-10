namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class NavigatorHomeRoomComposer : ServerPacket
{
    public NavigatorHomeRoomComposer(int homeRoomId, int roomIdToEnter)
        : base(ServerPacketHeader.USER_HOME_ROOM)
    {
        this.WriteInteger(homeRoomId);
        this.WriteInteger(roomIdToEnter);
    }
}