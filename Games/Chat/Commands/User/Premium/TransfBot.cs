namespace WibboEmulator.Games.Chat.Commands.User.Premium;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class TransfBot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.NONE || userRoom.InGame)
        {
            return;
        }

        if (!userRoom.IsTransf && !userRoom.IsSpectator)
        {
            userRoom.TransfBot = !userRoom.TransfBot;

            room.SendPacket(new UserRemoveComposer(userRoom.VirtualId));
            room.SendPacket(new UsersComposer(userRoom));
        }
    }
}
