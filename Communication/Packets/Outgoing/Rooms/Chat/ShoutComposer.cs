namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ShoutComposer : ServerPacket
    {
        public ShoutComposer(int VirtualId, string MessageText, int Color)
            : base(ServerPacketHeader.UNIT_CHAT_SHOUT)
        {
            this.WriteInteger(VirtualId);
            this.WriteString(MessageText);
            this.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(MessageText));
            this.WriteInteger(Color);
            this.WriteInteger(0);
            this.WriteInteger(-1);
        }
    }
}
