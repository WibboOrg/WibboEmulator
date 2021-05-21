namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class StopSoundComposer : ServerPacket
    {
        public StopSoundComposer(string Name)
            : base(22)
        {
            this.WriteString(Name);
        }
    }
}