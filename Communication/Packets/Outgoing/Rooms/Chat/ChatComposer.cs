namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat
{
    internal class ChatComposer : ServerPacket
    {
        public ChatComposer(int VirtualId, string Message, int Color)
            : base(ServerPacketHeader.UNIT_CHAT)
        {
            this.WriteInteger(VirtualId);
            this.WriteString(Message);
            this.WriteInteger(WibboEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message));
            this.WriteInteger(Color);
            this.WriteInteger(0);
            this.WriteInteger(-1);
        }
    }
}
