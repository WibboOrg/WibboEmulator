namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class AuthenticationOKComposer : ServerPacket
    {
        public AuthenticationOKComposer()
            : base(ServerPacketHeader.AUTHENTICATED)
        {

        }
    }
}
