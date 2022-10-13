namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;

using WibboEmulator.Communication.Packets.Outgoing.Televisions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Youtube : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        var username = parameters[1];
        var url = parameters[2];

        if (string.IsNullOrEmpty(url) || (!url.Contains("?v=") && !url.Contains("youtu.be/")))
        {
            return;
        }

        var split = "";

        if (url.Contains("?v="))
        {
            split = url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];
        }
        else if (url.Contains("youtu.be/"))
        {
            split = url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];
        }

        if (split.Length < 11)
        {
            return;
        }
        var videoId = split[..11];

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.Client == null || roomUserByUserId.Client.GetUser() == null)
        {
            return;
        }

        if (session.Langue != roomUserByUserId.Client.Langue)
        {
            session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue.user", session.Langue), roomUserByUserId.Client.Langue));
            return;
        }

        roomUserByUserId.Client.SendPacket(new YoutubeTvComposer(0, videoId));
    }
}
