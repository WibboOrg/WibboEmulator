using Wibbo.Communication.Packets.Outgoing.Avatar;
using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class ForceMimic : IChatCommand
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

            Room currentRoom = roomUserByUserId.Room;
            Client clientByUsername = roomUserByUserId.GetClient();
            if (currentRoom == null)
            {
                return;
            }

            clientByUsername.GetUser().Gender = Session.GetUser().Gender;
            clientByUsername.GetUser().Look = Session.GetUser().Look;

            if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
            {
                return;
            }

            if (!clientByUsername.GetUser().InRoom)
            {
                return;
            }

            clientByUsername.SendPacket(new FigureUpdateComposer(clientByUsername.GetUser().Look, clientByUsername.GetUser().Gender));
            clientByUsername.SendPacket(new UserChangeComposer(roomUserByUserId, true));
            currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));

        }
    }
}
