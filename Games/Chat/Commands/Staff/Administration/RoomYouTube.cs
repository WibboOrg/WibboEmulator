namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;

using WibboEmulator.Communication.Packets.Outgoing.Televisions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomYouTube : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var Url = parameters[1];

        if (string.IsNullOrEmpty(Url) || !Url.Contains("?v=") && !Url.Contains("youtu.be/")) //https://youtu.be/_mNig3ZxYbM
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

        Room.SendPacket(new YoutubeTvComposer(0, VideoId));
    }
}
