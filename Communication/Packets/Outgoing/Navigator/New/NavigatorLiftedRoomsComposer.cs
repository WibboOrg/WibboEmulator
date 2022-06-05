namespace Wibbo.Communication.Packets.Outgoing.Navigator.New
{
    internal class NavigatorLiftedRoomsComposer : ServerPacket
    {
        public NavigatorLiftedRoomsComposer()
            : base(ServerPacketHeader.NAVIGATOR_LIFTED)
        {
            this.WriteInteger(0);//Count
            {
                this.WriteInteger(1);//Flat Id
                this.WriteInteger(0);//Unknown
                this.WriteString("");//Image
                this.WriteString("Caption");//Caption.
            }
        }
    }
}
