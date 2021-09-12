using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class RoomMute : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Room.RoomMuted = !Room.RoomMuted;

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (User == null)
                {
                    continue;
                }

                User.SendWhisperChat((Room.RoomMuted) ? "Vous ne pouvez plus parler" : "Vous pouvez à présent parler");
            }
        }
    }
}
