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
            
            Client clientByUsername = roomUserByUserId.GetClient();
            if (clientByUsername.GetUser().SpectatorMode)
            {
                return;
            }

            string username = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);

            RoomUser roomUserByUserId = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByUserId == null)
            {
                return;
            }

            roomUserByUserId.OnChat(Message, 0, false);

        }
    }
}
