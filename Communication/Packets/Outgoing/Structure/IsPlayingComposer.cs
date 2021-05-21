namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class IsPlayingComposer : ServerPacket
    {
        public IsPlayingComposer(bool isPlaying)
            : base(ServerPacketHeader.PLAYING_GAME)
        {
            this.WriteBoolean(isPlaying);
        }
    }
}