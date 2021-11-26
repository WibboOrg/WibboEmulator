namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionErrorComposer : ServerPacket
    {
        public OnGuideSessionErrorComposer(int type)
            : base(ServerPacketHeader.OnGuideSessionError)
        {
            WriteInteger(type);
        }
    }
}
