using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Lay : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserId = UserRoom;
            if (roomUserByUserId == null)
            {
                return;
            }

            try
            {








                /*if (roomUserByUserId.sentadoBol)
                {
                  roomUserByUserId.sentadoBol = false;
                  roomUserByUserId.RemoveStatus("sit");
                }*/
                if (roomUserByUserId.Statusses.ContainsKey("lay") || roomUserByUserId.Statusses.ContainsKey("sit"))
                {
                    return;
                }

                if (roomUserByUserId.RotBody % 2 == 0 || roomUserByUserId.IsTransf)
                {
                    if (roomUserByUserId.RotBody == 4 || roomUserByUserId.RotBody == 0 || roomUserByUserId.IsTransf)
                    {
                        if (room.GetGameMap().CanWalk(roomUserByUserId.X, roomUserByUserId.Y + 1))
                        {
                            roomUserByUserId.RotBody = 0;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (!room.GetGameMap().CanWalk(roomUserByUserId.X + 1, roomUserByUserId.Y))
                        {
                            return;
                        }
                    }

                    //roomUserByUserId.AddStatus("lay", Convert.ToString((double) room.GetGameMap().Model.SqFloorHeight[roomUserByUserId.X, roomUserByUserId.Y] + 0.85).Replace(",", "."));
                    if (UserRoom.IsTransf)
                    {
                        roomUserByUserId.SetStatus("lay", "");
                    }
                    else
                    {
                        roomUserByUserId.SetStatus("lay", "0.7");
                    }

                    roomUserByUserId.IsLay = true;
                    roomUserByUserId.UpdateNeeded = true;
                }
            }
            catch
            {
            }

        }
    }
}
