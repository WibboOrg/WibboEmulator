namespace Wibbo.Communication.Packets.Outgoing.Navigator.New
{
    internal class NavigatorCollapsedCategoriesComposer : ServerPacket
    {
        public NavigatorCollapsedCategoriesComposer()
            : base(ServerPacketHeader.NAVIGATOR_COLLAPSED)
        {
            this.WriteInteger(0);
        }
    }
}
