using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ForceTransf : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string username = Params[1];

            RoomUser roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByUserId == null)
            {
                return;
            }

            GameClient clientByUsername = roomUserByUserId.GetClient();
            if (clientByUsername == null)
            {
                return;
            }

            if (clientByUsername.GetUser().SpectatorMode)
            {
                return;
            }

            if (Params.Length != 4 && Params.Length != 3)
            {
                return;
            }

            Room RoomClient = roomUserByUserId.GetClient().GetUser().CurrentRoom;
            if (RoomClient == null)
            {
                return;
            }

            int raceid = 0;
            if (Params.Length == 4)
            {
                string x = Params[3];
                if (int.TryParse(x, out int value))
                {
                    int.TryParse(Params[2], out raceid);
                    if (raceid < 1 || raceid > 50)
                    {
                        raceid = 0;
                    }
                }
            }
            else
            {
                raceid = 0;
            }

            if (!roomUserByUserId.SetPetTransformation(Params[2], raceid))
            {
                Session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", Session.Langue));
                return;
            }

            roomUserByUserId.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.transf.helpstop", Session.Langue));

            roomUserByUserId.IsTransf = true;

            RoomClient.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
            RoomClient.SendPacket(new UsersComposer(roomUserByUserId));
        }
    }
}
