using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class TransfStop : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            if (UserRoom.transformation && !UserRoom.IsSpectator && !UserRoom.InGame)            {                Room RoomClient = Session.GetHabbo().CurrentRoom;                if (RoomClient != null)                {                    UserRoom.transformation = false;                    RoomClient.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));                    RoomClient.SendPacket(new UsersComposer(UserRoom));                }            }        }    }}