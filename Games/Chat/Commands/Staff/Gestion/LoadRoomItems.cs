namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class LoadRoomItems : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (!int.TryParse(parameters[1], out var RoomId))
        {
            return;
        }

        if (Room.Id == RoomId)
        {
            return;
        }

        Room.GetRoomItemHandler().LoadFurniture(RoomId);
        Room.GetGameMap().GenerateMaps(true);
        session.SendWhisper("Mobi de l'appart n° " + RoomId + " chargé!");
        session.SendPacket(new GetGuestRoomResultComposer(session, Room.RoomData, false, true));
    }
}
