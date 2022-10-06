namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal class RespectNotificationComposer : ServerPacket
{
    public RespectNotificationComposer(int id, int respect)
        : base(ServerPacketHeader.USER_RESPECT)
    {
        this.WriteInteger(id);
        this.WriteInteger(respect);
    }
}
