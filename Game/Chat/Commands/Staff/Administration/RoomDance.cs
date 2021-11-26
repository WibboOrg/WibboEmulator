using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RoomDance : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                UserRoom.SendWhisperChat("Please enter a dance ID. (1-4)");
                return;
            }

            int DanceId = Convert.ToInt32(Params[1]);
            if (DanceId < 0 || DanceId > 4)
            {
                UserRoom.SendWhisperChat("Please enter a dance ID. (1-4)");
                return;
            }

            List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
            if (Users.Count > 0)
            {
                foreach (RoomUser U in Users.ToList())
                {
                    if (U == null)
                        continue;

                    if (U.CarryItemID > 0)
                        U.CarryItemID = 0;

                    U.DanceId = DanceId;
                    Room.SendPacket(new DanceComposer(U, DanceId));
                }
            }
        }
    }
}