using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class SetTransfStop : IChatCommand    {        public string PermissionRequired
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

            if (roomUserByHabbo.transformation && !roomUserByHabbo.IsSpectator)            {                Room RoomClient = roomUserByHabbo.Room;                if (RoomClient != null)                {                    roomUserByHabbo.transformation = false;                    RoomClient.SendPacket(new UserRemoveComposer(roomUserByHabbo.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(roomUserByHabbo));                }            }        }    }}