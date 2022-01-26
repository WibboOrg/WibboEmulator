namespace Butterfly.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ChatComposer : ServerPacket
    {
        public ChatComposer(int VirtualId, string MessageText, int Color)
            : base(ServerPacketHeader.UNIT_CHAT)
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
