namespace Butterfly.Communication.Packets.Outgoing.Custom
{
    internal class StopSoundComposer : ServerPacket
    {
        public StopSoundComposer(string Name)
            : base(ServerPacketHeader.STOP_SOUND)
        {
            this.WriteString(Name);
        }
    }
}