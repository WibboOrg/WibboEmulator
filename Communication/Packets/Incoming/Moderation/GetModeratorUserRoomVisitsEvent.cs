using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserRoomVisitsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_mod"))
            {
                return;
            }

            int userId = Packet.PopInt();

            Client clientTarget = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);

            if (clientTarget == null)
            {
                return;
            }

            Session.SendPacket(new ModeratorUserRoomVisitsComposer(clientTarget.GetUser(), clientTarget.GetUser().Visits));
        }
    }
}
