namespace WibboEmulator.Games.Chats.Commands.User.Premium;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class TransfBot : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
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
