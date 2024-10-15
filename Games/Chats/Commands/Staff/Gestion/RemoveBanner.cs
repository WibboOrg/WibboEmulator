namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RemoveBanner : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (TargetUser == null || TargetUser.User == null || TargetUser.User.BannerComponent == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
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

        if (!TargetUser.User.BannerComponent.BannerList.Contains(banner))
        {
            return;
        }

        var dbClient = DatabaseManager.Connection;
        TargetUser.User.BannerComponent.RemoveBanner(dbClient, bannerId);
    }
}
