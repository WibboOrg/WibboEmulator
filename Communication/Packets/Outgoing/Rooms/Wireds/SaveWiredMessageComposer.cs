namespace Butterfly.Communication.Packets.Outgoing.Rooms.Wireds
{
    internal class SaveWiredMessageComposer : ServerPacket
    {
        public SaveWiredMessageComposer()
            : base(ServerPacketHeader.WIRED_SAVE)
        {

        }
    }
}
