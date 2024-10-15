namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Database;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomBanner : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
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

        foreach (var TargetUser in room.RoomUserManager.UserList.ToList())
        {
            if (TargetUser == null || TargetUser.IsBot || TargetUser.Client == null || TargetUser.Client.User.BannerComponent == null)
            {
                continue;
            }

            TargetUser.Client.User.BannerComponent.AddBanner(dbClient, bannerId);
        }
    }
}
