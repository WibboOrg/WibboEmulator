namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionPartnerIsTypingComposer : ServerPacket
    {
        public OnGuideSessionPartnerIsTypingComposer()
            : base(ServerPacketHeader.OnGuideSessionPartnerIsTyping)
        {

        }
    }
}
