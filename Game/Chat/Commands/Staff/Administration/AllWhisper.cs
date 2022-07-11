using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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