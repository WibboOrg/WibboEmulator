using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GuildForumListEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.GetHabbo().SendWebPacket(new NavigateWebComposer("/forum/category/0"));
        }
    }
}