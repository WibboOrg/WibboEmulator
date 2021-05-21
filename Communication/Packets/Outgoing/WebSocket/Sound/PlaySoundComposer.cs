namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class PlaySoundComposer : ServerPacket
    {
        public PlaySoundComposer(string Name, int Type, bool Loop = false)
            : base(21)
        {
            this.WriteString(Name);
            this.WriteInteger(Type);
            this.WriteBoolean(Loop);
        }
    }
}
