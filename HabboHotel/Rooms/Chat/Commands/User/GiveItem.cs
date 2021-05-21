using Butterfly.HabboHotel.GameClients;using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class GiveItem : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo1 = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);            if (roomUserByHabbo1 == null || roomUserByHabbo1.CarryItemID <= 0 || roomUserByHabbo1.CarryTimer <= 0)
            {
                return;
            }

            RoomUser roomUserByHabbo2 = room.GetRoomUserManager().GetRoomUserByName(Params[1]);            if (roomUserByHabbo2 == null)
            {
                return;
            }

            if (Math.Abs(roomUserByHabbo1.X - roomUserByHabbo2.X) >= 3 || Math.Abs(roomUserByHabbo1.Y - roomUserByHabbo2.Y) >= 3)
            {
                return;
            }

            roomUserByHabbo2.CarryItem(roomUserByHabbo1.CarryItemID);            roomUserByHabbo1.CarryItem(0);        }    }}