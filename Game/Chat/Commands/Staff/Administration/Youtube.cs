using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;
using System;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Youtube : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
            {
                return;
            }

            string username = Params[1];
            string Url = Params[2];

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

            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null || roomUserByHabbo.GetClient().GetHabbo() == null)
            {
                return;
            }

            if (Session.Langue != roomUserByHabbo.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByHabbo.GetClient().Langue), Session.Langue));
                return;
            }

            roomUserByHabbo.GetClient().GetHabbo().SendWebPacket(new YoutubeTvComposer(0, VideoId));
        }
    }
}
