using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms.Games;

namespace WibboEmulator.Games.Chat.Commands.Cmd
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
                UserRoom.IsTransf = false;

                Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
                Room.SendPacket(new UsersComposer(UserRoom));
            }
        }
    }
}
