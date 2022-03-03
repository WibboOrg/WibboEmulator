using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class ForceTransfStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];

            RoomUser roomUserByUserId = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByUserId == null)
            {
                return;
            }

            if (roomUserByUserId.transformation && !roomUserByUserId.IsSpectator)
            {
                Room RoomClient = roomUserByUserId.Room;
                if (RoomClient != null)
                {
                    roomUserByUserId.transformation = false;

                    RoomClient.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(roomUserByUserId));
                }
            }

        }
    }
}
