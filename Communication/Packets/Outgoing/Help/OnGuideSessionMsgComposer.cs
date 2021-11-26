namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionMsgComposer : ServerPacket
    {
        public OnGuideSessionMsgComposer(string message, int userId)
            : base(ServerPacketHeader.OnGuideSessionMsg)
        {
            WriteString(message);
            WriteInteger(userId);
        }
    }
}
