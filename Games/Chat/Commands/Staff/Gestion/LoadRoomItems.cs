namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class LoadRoomItems : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
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

        room.GetRoomItemHandler().LoadFurniture(roomId);
        room.GetGameMap().GenerateMaps(true);
        session.SendWhisper("Mobi de l'appart n° " + roomId + " chargé!");
        session.SendPacket(new GetGuestRoomResultComposer(session, room.RoomData, false, true));
    }
}
