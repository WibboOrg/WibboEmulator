namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class NavigatorFlatCatsMessageComposer : ServerPacket
    {
        public NavigatorFlatCatsMessageComposer()
            : base(ServerPacketHeader.NAVIGATOR_EVENT_CATEGORIES)
        {

        }
    }
}
