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

        foreach (var item_0 in room.RoomUserManager.GetUserList().ToList())
        {
            try
            {
                if (!item_0.IsBot)
                {
                    if (item_0.Client != null)
                    {
                        if (item_0.Client.GetUser() != null)
                        {
                            item_0.Client.GetUser().GetBadgeComponent().GiveBadge(badgeId, true);
                            item_0.Client.SendPacket(new ReceiveBadgeComposer(badgeId));
                        }
                    }
                }
            }
            catch
            {
            }
        }

    }
}
