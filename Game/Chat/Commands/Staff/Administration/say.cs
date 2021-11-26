using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Say : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
            {
                return;
            }

            string username = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);

            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByHabbo == null)
            {
                return;
            }

            roomUserByHabbo.OnChat(Message, 0, false);

        }
    }
}
