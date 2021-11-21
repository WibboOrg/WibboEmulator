using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Clients;namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class SetCopyLook : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);            if (roomUserByHabbo == null)
            {
                return;
            }

            Room currentRoom = roomUserByHabbo.Room;            Client clientByUsername = roomUserByHabbo.GetClient();            if (currentRoom == null)
            {
                return;
            }

            clientByUsername.GetHabbo().Gender = Session.GetHabbo().Gender;            clientByUsername.GetHabbo().Look = Session.GetHabbo().Look;            if (roomUserByHabbo.transformation || roomUserByHabbo.IsSpectator)
            {
                return;
            }

            if (!clientByUsername.GetHabbo().InRoom)
            {
                return;
            }

            clientByUsername.SendPacket(new UserChangeComposer(roomUserByHabbo, true));            currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));        }    }}