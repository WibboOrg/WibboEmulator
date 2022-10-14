namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomBadge : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var badgeId = parameters[1];

        foreach (var user in room.RoomUserManager.GetUserList().ToList())
        {
            if (!user.IsBot)
            {
                if (user.Client != null)
                {
                    if (user.Client.GetUser() != null)
                    {
                        user.Client.GetUser().BadgeComponent.GiveBadge(badgeId, true);
                        user.Client.SendPacket(new ReceiveBadgeComposer(badgeId));
                    }
                }
            }
        }

    }
}
