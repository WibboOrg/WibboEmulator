using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class SuperPull : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = Session.GetUser().CurrentRoom;
            if (room == null)
            {
                return;
            }

            RoomUser roomuser = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomuser == null)
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            RoomUser TargetUser = room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (TargetUser == null)
            {
                return;
            }

            if (TargetUser.GetClient().GetUser().Id == Session.GetUser().Id)
            {
                return;
            }

            if (TargetUser.GetClient().GetUser().PremiumProtect && !Session.GetUser().HasFuse("fuse_mod"))
            {
                roomuser.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                return;
            }

            RoomUser TUS = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (TUS == null)
                return;

            if (TUS.SetX - 1 == Room.GetGameMap().Model.DoorX)
            {
                return;
            }

            roomuser.OnChat("*Tire " + Params[1] + "*", 0, false);
            if (roomuser.RotBody % 2 != 0)
            {
                roomuser.RotBody--;
            }

            if (roomuser.RotBody == 0)
            {
                TargetUser.MoveTo(roomuser.X, roomuser.Y - 1);
            }
            else if (roomuser.RotBody == 2)
            {
                TargetUser.MoveTo(roomuser.X + 1, roomuser.Y);
            }
            else if (roomuser.RotBody == 4)
            {
                TargetUser.MoveTo(roomuser.X, roomuser.Y + 1);
            }
            else if (roomuser.RotBody == 6)
            {
                TargetUser.MoveTo(roomuser.X - 1, roomuser.Y);
            }
        }
    }
}
