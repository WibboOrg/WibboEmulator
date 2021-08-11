namespace Butterfly.Communication.Packets.Outgoing.Navigator.New
{
    internal class NavigatorPreferencesComposer : ServerPacket
    {
        public NavigatorPreferencesComposer()
            : base(ServerPacketHeader.NAVIGATOR_SETTINGS)
        {
            this.WriteInteger(68);//X
            this.WriteInteger(42);//Y
            this.WriteInteger(425);//Width
            this.WriteInteger(592);//Height
            this.WriteBoolean(false);//Show or hide saved searches.
            this.WriteInteger(0);//No idea?
        }
    }
}
