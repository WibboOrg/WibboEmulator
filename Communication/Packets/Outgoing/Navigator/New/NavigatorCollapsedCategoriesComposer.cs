namespace WibboEmulator.Communication.Packets.Outgoing.Navigator.New;

internal sealed class NavigatorCollapsedCategoriesComposer : ServerPacket
{
    public NavigatorCollapsedCategoriesComposer()
        : base(ServerPacketHeader.NAVIGATOR_COLLAPSED) => this.WriteInteger(0);
}
