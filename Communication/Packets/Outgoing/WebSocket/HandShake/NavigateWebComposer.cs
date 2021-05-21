namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class NavigateWebComposer : ServerPacket
    {
        public NavigateWebComposer(string Path)
            : base(25)
        {
            this.WriteString(Path);
        }
    }
}
