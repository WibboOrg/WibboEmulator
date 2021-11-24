namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionMsgComposer : ServerPacket
    {
        public OnGuideSessionMsgComposer()
            : base(ServerPacketHeader.OnGuideSessionMsg)
        {

        }
    }
}
