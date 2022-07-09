using Wibbo.Communication.Packets.Outgoing.Rooms.Chat;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class AllWhisper : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
            {
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.GetClient() == null)
                {
                    continue;
                }
                User.GetClient().SendPacket(new WhisperComposer(UserRoom.VirtualId, Message, 0));
            }
        }
    }
}