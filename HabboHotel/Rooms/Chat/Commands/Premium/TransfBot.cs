using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class TransfBot : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            if (!UserRoom.transformation && !UserRoom.IsSpectator)            {                Room RoomClient = Session.GetHabbo().CurrentRoom;                if (RoomClient != null)                {                    UserRoom.transfbot = !UserRoom.transfbot;

                    RoomClient.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));                    RoomClient.SendPacket(new UsersComposer(UserRoom));                }            }        }    }}