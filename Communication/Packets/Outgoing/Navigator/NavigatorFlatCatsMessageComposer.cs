namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorFlatCatsMessageComposer : ServerPacket
    {
        public NavigatorFlatCatsMessageComposer()
            : base(ServerPacketHeader.NAVIGATOR_EVENT_CATEGORIES)
        {

        }
    }
}
