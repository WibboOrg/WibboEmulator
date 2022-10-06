namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MassBadge : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var Badge = parameters[1];

        if (string.IsNullOrEmpty(Badge))
        {
            return;
        }

        foreach (var Client in WibboEnvironment.GetGame().GetGameClientManager().GetClients.ToList())
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
