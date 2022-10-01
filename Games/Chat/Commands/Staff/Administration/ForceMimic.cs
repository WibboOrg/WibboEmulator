using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ForceMimic : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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

            Room currentRoom = roomUserByUserId.Room;
            GameClient clientByUsername = roomUserByUserId.GetClient();
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
