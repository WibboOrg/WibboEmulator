using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class TransfStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (UserRoom.transformation && !UserRoom.IsSpectator && !UserRoom.InGame)
            {
                Room RoomClient = Session.GetUser().CurrentRoom;
                if (RoomClient != null)
                {
                    UserRoom.transformation = false;

                    RoomClient.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(UserRoom));
                }
            }

        }
    }
}
