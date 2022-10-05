namespace WibboEmulator.Games.Chat.Commands.Cmd;

using WibboEmulator.Communication.Packets.Outgoing.Televisions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Youtube : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 3)
        {
            return;
        }

        var username = Params[1];
        var Url = Params[2];

        if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
        {
            return;
        }

        var Split = "";

        if (Url.Contains("?v="))
        {
            Split = Url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];
        }
        else if (Url.Contains("youtu.be/"))
        {
            Split = Url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];
        }

        if (Split.Length < 11)
        {
            return;
        }
        var VideoId = Split[..11];

        var roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.GetClient() == null || roomUserByUserId.GetClient().GetUser() == null)
        {
            return;
        }

        if (session.Langue != roomUserByUserId.GetClient().Langue)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), session.Langue));
            return;
        }

        roomUserByUserId.GetClient().SendPacket(new YoutubeTvComposer(0, VideoId));
    }
}
