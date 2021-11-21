using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class OpenHelpToolEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.OpenHelpToolMessageComposer);
            serverMessage.WriteInteger(0);
            Session.SendPacket(serverMessage);
        }
    }
}
