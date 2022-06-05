namespace Wibbo.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorFlatCatsComposer : ServerPacket
    {
        public NavigatorFlatCatsComposer()
            : base(ServerPacketHeader.NAVIGATOR_EVENT_CATEGORIES)
        {

        }
    }
}
