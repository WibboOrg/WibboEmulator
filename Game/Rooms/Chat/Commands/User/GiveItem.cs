using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            {
                return;
            }

            RoomUser roomUserByHabbo1 = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            {
                return;
            }

            RoomUser roomUserByHabbo2 = room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            {
                return;
            }

            if (Math.Abs(roomUserByHabbo1.X - roomUserByHabbo2.X) >= 3 || Math.Abs(roomUserByHabbo1.Y - roomUserByHabbo2.Y) >= 3)
            {
                return;
            }

            roomUserByHabbo2.CarryItem(roomUserByHabbo1.CarryItemID);