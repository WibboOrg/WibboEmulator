namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class LoadRoomItems : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var roomId))
        {
            return;
        }

        if (room.Id == roomId)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        room.RoomItemHandling.LoadFurniture(dbClient, roomId);
        room.GameMap.GenerateMaps(true);
        Session.SendWhisper("Mobi de l'appart n° " + roomId + " chargé!");
        Session.SendPacket(new GetGuestRoomResultComposer(Session, room.RoomData, false, true));
    }
}
