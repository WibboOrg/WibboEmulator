namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveBanner : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        var targetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null || targetUser.User.BannerComponent == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", session.Language));
            return;
        }

        if (!int.TryParse(parameters[2], out var bannerId))
        {
            return;
        }

        if (!BannerManager.TryGetBannerById(bannerId, out var banner))
        {
            return;
        }

        if (targetUser.User.BannerComponent.BannerList.Contains(banner))
        {
            return;
        }

        var dbClient = DatabaseManager.Connection;
        targetUser.User.BannerComponent.AddBanner(dbClient, bannerId);

        userRoom.SendWhisperChat(LanguageManager.TryGetValue("command.givebanner.success", session.Language));
    }
}
