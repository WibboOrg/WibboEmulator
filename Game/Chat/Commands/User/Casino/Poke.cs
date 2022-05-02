using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Poke : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 4)
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
            int.TryParse(Params[2], out int DiceCount);
            int.TryParse(Params[3], out int WpCount);

            if (string.IsNullOrWhiteSpace(Username))
            {
                return;
            }

            if(DiceCount > 5 || DiceCount < 1)
            {
                return;
            }

            if(WpCount > Session.GetUser().WibboPoints)
            {
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByName(Username);
            if(TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetUser() == null)
            {
                return;
            }

            if(WpCount > TargetUser.GetClient().GetUser().WibboPoints)
            {
                return;
            }


        }
    }
}
