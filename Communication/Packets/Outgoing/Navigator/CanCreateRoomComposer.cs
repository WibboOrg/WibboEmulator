namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal class CanCreateRoomComposer : ServerPacket
{
    public CanCreateRoomComposer(bool Error, int MaxRoomsPerUser)
        : base(ServerPacketHeader.CAN_CREATE_ROOM_MESSAGE_COMPOSER)
    {
        this.WriteInteger(Error ? 1 : 0);
        this.WriteInteger(MaxRoomsPerUser);
    }
}
