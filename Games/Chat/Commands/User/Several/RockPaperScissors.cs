using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Rooms.Jankens;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class RockPaperScissors : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (Session.GetUser().SpectatorMode)
            {
                return;
            }

            string Username = Params[1];

            if (string.IsNullOrWhiteSpace(Username))
            {
                return;
            }

            RoomUser roomUserTarget = Room.GetRoomUserManager().GetRoomUserByName(Username);
            if (roomUserTarget == null)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            if (roomUserTarget.UserId == UserRoom.UserId)
            {
                return;
            }

            JankenManager Jankan = Room.GetJanken();
            Jankan.Start(UserRoom, roomUserTarget);
        }
    }
}
