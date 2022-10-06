namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal class DoorbellComposer : ServerPacket
{
    public DoorbellComposer(string username)
        : base(ServerPacketHeader.ROOM_DOORBELL) => this.WriteString(username);
}
