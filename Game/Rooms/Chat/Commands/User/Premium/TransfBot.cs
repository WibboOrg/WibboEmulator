using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.GameClients;using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class TransfBot : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            if (!UserRoom.transformation && !UserRoom.IsSpectator)            {                Room RoomClient = Session.GetHabbo().CurrentRoom;                if (RoomClient != null)                {                    UserRoom.transfbot = !UserRoom.transfbot;

                    RoomClient.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));                    RoomClient.SendPacket(new UsersComposer(UserRoom));                }            }        }    }}