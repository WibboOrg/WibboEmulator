namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.session;

internal class CloseConnectionComposer : ServerPacket
{
    public CloseConnectionComposer()
        : base(ServerPacketHeader.DESKTOP_VIEW)
    {

    }
}
