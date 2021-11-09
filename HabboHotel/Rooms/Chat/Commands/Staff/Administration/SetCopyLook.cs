using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class SetCopyLook : IChatCommand    {        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);            if (roomUserByHabbo == null)
            {
                return;
            }

            Room currentRoom = roomUserByHabbo.Room;            GameClient clientByUsername = roomUserByHabbo.GetClient();            if (currentRoom == null)
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