namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OnGuideSessionAttachedComposer : ServerPacket
    {
        public OnGuideSessionAttachedComposer(bool enable, int userId, string mesage, int time)
            : base(ServerPacketHeader.GUIDE_SESSION_ATTACHED)
        {
            WriteBoolean(enable);
            WriteInteger(userId);
            WriteString(mesage);
            WriteInteger(time);
        }
    }
}
