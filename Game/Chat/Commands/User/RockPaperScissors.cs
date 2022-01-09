using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Jankens;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RockPaperScissors : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (Session.GetHabbo().SpectatorMode)
            {
                return;
            }

            string Username = Params[1];

            if (string.IsNullOrWhiteSpace(Username))
            {
                return;
            }

            Room room = UserRoom.Room;
            if (room == null)
            {
                return;
            }

            RoomUser roomuser = room.GetRoomUserManager().GetRoomUserByName(Username);
            if (roomuser == null)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            if (roomuser.UserId == UserRoom.UserId)
            {
                return;
            }

            JankenManager Jankan = room.GetJanken();
            Jankan.Start(UserRoom, roomuser);
        }
    }
}
