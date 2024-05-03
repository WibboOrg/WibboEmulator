namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Database;
using WibboEmulator.Games.Banners;
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

        if (!BannerManager.TryGetBannerById(bannerId, out var banner))
        {
            return;
        }

        var dbClient = DatabaseManager.Connection;

        foreach (var targetUser in room.RoomUserManager.UserList.ToList())
        {
            if (targetUser == null || targetUser.IsBot || targetUser.Client == null || targetUser.Client.User.BannerComponent == null)
            {
                continue;
            }

            targetUser.Client.User.BannerComponent.AddBanner(dbClient, bannerId);
        }
    }
}
