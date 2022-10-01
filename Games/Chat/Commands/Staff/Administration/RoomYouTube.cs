
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Communication.Packets.Outgoing.Televisions;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class RoomYouTube : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            string Url = Params[1];

            if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
            {
                return;
            }

            string Split = "";

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
            string VideoId = Split.Substring(0, 11);

            Room.SendPacket(new YoutubeTvComposer(0, VideoId));
        }
    }
}
