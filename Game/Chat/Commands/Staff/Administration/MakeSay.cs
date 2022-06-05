using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class MakeSay : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
            {
                return;
            }
            
            if (UserRoom.GetClient().GetUser().SpectatorMode)
            {
                return;
            }

            string username = Params[1];
            string message = CommandManager.MergeParams(Params, 2);

            RoomUser roomUserByUserId = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByUserId == null)
            {
                return;
            }

            roomUserByUserId.OnChat(message, 0, false);

        }
    }
}
