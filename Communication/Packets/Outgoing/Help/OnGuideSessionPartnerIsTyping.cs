namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionPartnerIsTyping : ServerPacket
    {
        public OnGuideSessionPartnerIsTyping()
            : base(ServerPacketHeader.OnGuideSessionPartnerIsTyping)
        {

        }
    }
}
