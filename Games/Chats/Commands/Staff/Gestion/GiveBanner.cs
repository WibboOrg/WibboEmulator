namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;

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

        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);
        if (targetUser == null || targetUser.User == null || targetUser.User.Banner == null)
        {
            userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
            return;
        }

        if (!int.TryParse(parameters[2], out var bannerId))
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetBannerManager().TryGetBannerById(bannerId, out var banner))
        {
            return;
        }

        if (targetUser.User.Banner.BannerList.Contains(banner))
        {
            return;
        }

        var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        targetUser.User.Banner.AddBanner(dbClient, bannerId);

        userRoom.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("command.givebanner.success", session.Langue));
    }
}
