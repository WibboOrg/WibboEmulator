namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class TransfStop : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
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
