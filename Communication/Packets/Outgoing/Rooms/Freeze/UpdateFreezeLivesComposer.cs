namespace Butterfly.Communication.Packets.Outgoing.Rooms.Freeze
{
    internal class UpdateFreezeLivesComposer : ServerPacket
    {
        public UpdateFreezeLivesComposer()
            : base(ServerPacketHeader.UNIT_NUMBER)
        {

        }
    }
}
