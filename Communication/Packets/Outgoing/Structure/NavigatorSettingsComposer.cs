namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int Homeroom)
            : base(ServerPacketHeader.USER_HOME_ROOM)
        {
            this.WriteInteger(Homeroom);
            this.WriteInteger(Homeroom);
        }
    }
}
