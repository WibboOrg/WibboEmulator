using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using Butterfly.Game.User;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UnIgnoreUserEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().CurrentRoom == null)
            {
                return;
            }

            string str = Packet.PopString();

            Habbo habbo = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(str).GetHabbo();
            if (habbo == null || !Session.GetHabbo().MutedUsers.Contains(habbo.Id))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Remove(habbo.Id);

            ServerPacket Response = new ServerPacket(ServerPacketHeader.USER_IGNORED_UPDATE);
            Response.WriteInteger(3);
            Response.WriteString(str);
            Session.SendPacket(Response);
        }
    }
}
