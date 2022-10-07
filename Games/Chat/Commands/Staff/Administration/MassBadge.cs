namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MassBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var badge = parameters[1];

        if (string.IsNullOrEmpty(badge))
        {
            return;
        }

        foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetClients.ToList())
        {
            if (client.GetUser() != null)
            {
                client.GetUser().GetBadgeComponent().GiveBadge(badge, true);
                client.SendPacket(new ReceiveBadgeComposer(badge));
                client.SendNotification("Vous venez de recevoir le badge : " + badge + " !");
            }
        }
    }
}
