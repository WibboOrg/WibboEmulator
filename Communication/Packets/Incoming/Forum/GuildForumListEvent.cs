using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GuildForumListEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.GetUser().SendWebPacket(new NavigateWebComposer("/forum/category/0"));
        }
    }
}