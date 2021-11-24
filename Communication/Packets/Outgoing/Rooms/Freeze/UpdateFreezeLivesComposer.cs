namespace Butterfly.Communication.Packets.Outgoing.Rooms.Freeze
{
    internal class UpdateFreezeLives : ServerPacket
    {
        public UpdateFreezeLives()
            : base(ServerPacketHeader.UNIT_NUMBER)
        {

        }
    }
}
