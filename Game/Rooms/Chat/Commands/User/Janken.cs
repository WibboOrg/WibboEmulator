using Butterfly.Game.GameClients;using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Janken;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class Janken : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            if (Session.GetHabbo().SpectatorMode)
            {
                return;
            }

            string Username = Params[1];            if (string.IsNullOrWhiteSpace(Username))
            {
                return;
            }

            Room room = UserRoom.Room;            if (room == null)
            {
                return;
            }

            RoomUser roomuser = room.GetRoomUserManager().GetRoomUserByName(Username);            if (roomuser == null)            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));                return;            }            if (roomuser.UserId == UserRoom.UserId)
            {
                return;
            }

            JankenManager Jankan = room.GetJanken();            Jankan.Duel(UserRoom, roomuser);        }    }}