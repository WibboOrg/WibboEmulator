namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class ActionComposer : ServerPacket
    {
        public ActionComposer(int VirtualId, int ActionId)
            : base(ServerPacketHeader.UNIT_EXPRESSION)
        {
            this.WriteInteger(VirtualId);
            this.WriteInteger(ActionId);
        }
    }
}
