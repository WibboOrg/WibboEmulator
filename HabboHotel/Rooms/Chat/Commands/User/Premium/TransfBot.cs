using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class TransfBot : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            if (!UserRoom.transformation && !UserRoom.IsSpectator)            {                Room RoomClient = Session.GetHabbo().CurrentRoom;                if (RoomClient != null)                {                    UserRoom.transfbot = !UserRoom.transfbot;

                    RoomClient.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));                    RoomClient.SendPacket(new UsersComposer(UserRoom));                }            }        }    }}