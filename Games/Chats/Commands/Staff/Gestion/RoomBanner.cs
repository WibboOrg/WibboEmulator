namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomBanner : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var bannerId))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetBannerManager().TryGetBannerById(bannerId, out var banner))
        {
            return;
        }

        var dbClient = WibboEnvironment.GetDatabaseManager().Connection();

        foreach (var targetUser in room.RoomUserManager.GetUserList().ToList())
        {
            if (targetUser == null || targetUser.IsBot || targetUser.Client == null || targetUser.Client.User.Banner == null)
            {
                continue;
            }

            targetUser.Client.User.Banner.AddBanner(dbClient, bannerId);
        }
    }
}
