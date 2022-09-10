using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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

            RoomUser roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByUserId == null)
            {
                return;
            }

            if (roomUserByUserId.IsTransf && !roomUserByUserId.IsSpectator)
            {
                Room RoomClient = roomUserByUserId.Room;
                if (RoomClient != null)
                {
                    roomUserByUserId.IsTransf = false;

                    RoomClient.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(roomUserByUserId));
                }
            }
        }
    }
}
