namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MassBadge : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var Badge = Params[1];

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
