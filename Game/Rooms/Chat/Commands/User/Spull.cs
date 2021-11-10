using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class Spull : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            Room room = Session.GetHabbo().CurrentRoom;            if (room == null)
            {
                return;
            }

            RoomUser roomuser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);            if (roomuser == null)
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            RoomUser TargetUser = room.GetRoomUserManager().GetRoomUserByName(Params[1]);            if (TargetUser == null)
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))            {                roomuser.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));                return;            }            roomuser.OnChat("*Tire " + Params[1] + "*", 0, false);            if (roomuser.RotBody % 2 != 0)
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
            }        }    }}