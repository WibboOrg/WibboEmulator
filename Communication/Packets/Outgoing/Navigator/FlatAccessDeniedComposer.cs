namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class FlatAccessDeniedComposer : ServerPacket
{
    public FlatAccessDeniedComposer(string username)
        : base(ServerPacketHeader.ROOM_DOORBELL_REJECTED)
    {
        if (username != null)
        {
            this.WriteString(username);
        }
    }
}
