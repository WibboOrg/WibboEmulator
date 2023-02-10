namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;

internal sealed class CloseConnectionComposer : ServerPacket
{
    public CloseConnectionComposer()
        : base(ServerPacketHeader.DESKTOP_VIEW)
    {

    }
}
