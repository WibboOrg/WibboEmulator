namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class CanCreateRoomComposer : ServerPacket
{
    public CanCreateRoomComposer(bool error, int maxRoomsPerUser)
        : base(ServerPacketHeader.CAN_CREATE_ROOM_MESSAGE_COMPOSER)
    {
        this.WriteInteger(error ? 1 : 0);
        this.WriteInteger(maxRoomsPerUser);
    }
}
