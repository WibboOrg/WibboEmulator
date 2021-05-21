namespace Butterfly.Communication.Packets.Outgoing.Structure
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
