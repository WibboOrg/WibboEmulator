namespace Butterfly.Communication.Packets.Outgoing.Misc
{
    internal class NuxAlertComposer : ServerPacket
    {
        public NuxAlertComposer(int type)
            : base(ServerPacketHeader.NuxAlertComposer)
        {
            this.WriteInteger(type);
        }
    }
}
