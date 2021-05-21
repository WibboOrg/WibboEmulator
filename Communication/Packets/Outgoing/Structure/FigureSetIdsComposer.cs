namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FigureSetIdsComposer : ServerPacket
    {
        public FigureSetIdsComposer()
            : base(ServerPacketHeader.USER_CLOTHING)
        {
            this.WriteInteger(0);

            this.WriteInteger(0);
        }
    }
}
