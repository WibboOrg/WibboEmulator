using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class SetTransfStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];

            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByHabbo == null)
            {
                return;
            }

            if (roomUserByHabbo.transformation && !roomUserByHabbo.IsSpectator)
            {
                Room RoomClient = roomUserByHabbo.Room;
                if (RoomClient != null)
                {
                    roomUserByHabbo.transformation = false;

                    RoomClient.SendPacket(new UserRemoveComposer(roomUserByHabbo.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(roomUserByHabbo));
                }
            }

        }
    }
}
