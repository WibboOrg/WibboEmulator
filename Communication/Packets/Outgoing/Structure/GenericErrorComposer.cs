namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class GenericErrorComposer : ServerPacket
    {
        public GenericErrorComposer(int errorId)
            : base(ServerPacketHeader.GENERIC_ERROR)
        {
            this.WriteInteger(errorId);
        }
    }
}
