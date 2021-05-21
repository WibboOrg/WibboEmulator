namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class UpdateFreezeLives : ServerPacket
    {
        public UpdateFreezeLives()
            : base(ServerPacketHeader.UNIT_NUMBER)
        {

        }
    }
}
