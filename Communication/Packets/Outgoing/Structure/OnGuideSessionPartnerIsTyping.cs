namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OnGuideSessionPartnerIsTyping : ServerPacket
    {
        public OnGuideSessionPartnerIsTyping()
            : base(ServerPacketHeader.OnGuideSessionPartnerIsTyping)
        {

        }
    }
}
