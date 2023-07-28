namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomBanner : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        if (!int.TryParse(parameters[2], out var bannerId))
        {
            return;
        }

        var banner = WibboEnvironment.GetGame().GetBannerManager().GetBannerById(bannerId);

        if (banner == null)
        {
            return;
        }

        var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        foreach (var targetUser in room.RoomUserManager.GetUserList().ToList())
        {
            if (targetUser == null || targetUser.IsBot || targetUser.Client == null || targetUser.Client.User.Banner == null)
            {
                return;
            }

            targetUser.Client.User.Banner.AddBanner(dbClient, bannerId);
        }
    }
}
