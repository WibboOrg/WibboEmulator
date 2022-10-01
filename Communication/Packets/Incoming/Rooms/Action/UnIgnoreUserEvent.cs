using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class UnIgnoreUserEvent : IPacketEvent
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

            string str = Packet.PopString();

            User user = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(str).GetUser();
            if (user == null || !Session.GetUser().MutedUsers.Contains(user.Id))
            {
                return;
            }

            Session.GetUser().MutedUsers.Remove(user.Id);

            Session.SendPacket(new IgnoreStatusComposer(3, str));
        }
    }
}
