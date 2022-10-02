using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class IgnoreUserEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            if (Session.GetUser().CurrentRoom == null)
            {
                return;
            }

            string UserName = Packet.PopString();

            GameClient gameclient = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(UserName);
            if (gameclient == null)
            {
                return;
            }

            User user = gameclient.GetUser();
            if (user == null || Session.GetUser().MutedUsers.Contains(user.Id))
            {
                return;
            }

            Session.GetUser().MutedUsers.Add(user.Id);

            Session.SendPacket(new IgnoreStatusComposer(1, UserName));
        }
    }
}