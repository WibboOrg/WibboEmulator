using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using Butterfly.Game.User;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class IgnoreUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().CurrentRoom == null)
            {
                return;
            }

            string UserName = Packet.PopString();

            GameClient gameclient = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(UserName);
            if (gameclient == null)
            {
                return;
            }

            Habbo habbo = gameclient.GetHabbo();
            if (habbo == null || Session.GetHabbo().MutedUsers.Contains(habbo.Id))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Add(habbo.Id);

            ServerPacket Response = new ServerPacket(ServerPacketHeader.USER_IGNORED_UPDATE);
            Response.WriteInteger(1);
            Response.WriteString(UserName);
            Session.SendPacket(Response);
        }
    }
}