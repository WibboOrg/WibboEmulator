using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms.Games;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class TransfStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            if (UserRoom.IsTransf && !UserRoom.IsSpectator && !UserRoom.InGame)
            {
                Room RoomClient = Session.GetUser().CurrentRoom;
                if (RoomClient != null)
                {
                    UserRoom.IsTransf = false;

                    RoomClient.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(UserRoom));
                }
            }

        }
    }
}
