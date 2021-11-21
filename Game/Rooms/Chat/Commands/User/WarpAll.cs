using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class WarpAll : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {            Room currentRoom = Session.GetHabbo().CurrentRoom;            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null)
            {
                return;
            }

            List<ServerPacket> MessageList = new List<ServerPacket>();            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())            {                if (user == null || user.IsBot)
                {
                    continue;
                }

                MessageList.Add(room.GetRoomItemHandler().TeleportUser(user, UserRoom.Coordinate, 0, room.GetGameMap().SqAbsoluteHeight(UserRoom.X, UserRoom.Y)));            }            room.SendMessage(MessageList);        }    }}