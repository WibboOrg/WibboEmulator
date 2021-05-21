namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class ActionMessageComposer : ServerPacket
    {
        public ActionMessageComposer(int VirtualId, int ActionId)
            : base(ServerPacketHeader.UNIT_EXPRESSION)
        {
            this.WriteInteger(VirtualId);
            this.WriteInteger(ActionId);
        }
    }
}
