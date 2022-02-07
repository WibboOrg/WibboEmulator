using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using System.Collections.Generic;
using System.Linq;
using Butterfly.Game.Rooms;
using Butterfly.Utility;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class AllWarp : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            ServerPacketList MessageList = new ServerPacketList();

            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (user == null || user.IsBot)
                {
                    continue;
                }

                MessageList.Add(room.GetRoomItemHandler().TeleportUser(user, UserRoom.Coordinate, 0, room.GetGameMap().SqAbsoluteHeight(UserRoom.X, UserRoom.Y)));
            }

            room.SendMessage(MessageList);
        }
    }
}
