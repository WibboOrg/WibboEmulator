namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;

internal sealed class NavigatorFlatCatsComposer : ServerPacket
{
    public NavigatorFlatCatsComposer()
        : base(ServerPacketHeader.NAVIGATOR_EVENT_CATEGORIES)
    {

    }
}
