namespace WibboEmulator.Games.Chat.Commands.User.Room;

using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Unload : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var usersToReturn = room.GetRoomUserManager().GetRoomUsers().ToList();

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        foreach (var user in usersToReturn)
        {
            if (user == null || user.Client == null)
            {
                continue;
            }

            user.Client.SendPacket(new RoomForwardComposer(room.Id));
        }
    }
}
