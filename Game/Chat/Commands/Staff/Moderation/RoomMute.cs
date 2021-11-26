using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RoomMute : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.RoomMuted = !Room.RoomMuted;

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (User == null)
                {
                    continue;
                }

                User.SendWhisperChat((Room.RoomMuted) ? "Vous ne pouvez plus parler" : "Vous pouvez � pr�sent parler");
            }
        }
    }
}
