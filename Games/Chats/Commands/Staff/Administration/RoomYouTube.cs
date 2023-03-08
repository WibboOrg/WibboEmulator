namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Communication.Packets.Outgoing.Televisions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomYouTube : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var url = parameters[1];

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

        room.SendPacket(new YoutubeTvComposer(0, videoId));
    }
}
