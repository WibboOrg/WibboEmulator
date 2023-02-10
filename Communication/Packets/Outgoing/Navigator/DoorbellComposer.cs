namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class DoorbellComposer : ServerPacket
{
    public DoorbellComposer(string username)
        : base(ServerPacketHeader.ROOM_DOORBELL) => this.WriteString(username);
}
