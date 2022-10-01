using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class MassBadge : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Badge = Params[1];

            if (string.IsNullOrEmpty(Badge))
            {
                return;
            }

            foreach (GameClient Client in WibboEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client.GetUser() != null)
                {
                    Client.GetUser().GetBadgeComponent().GiveBadge(Badge, true);
                    Client.SendPacket(new ReceiveBadgeComposer(Badge));
                    Client.SendNotification("Vous venez de recevoir le badge : " + Badge + " !");
                }
            }
        }
    }
}
