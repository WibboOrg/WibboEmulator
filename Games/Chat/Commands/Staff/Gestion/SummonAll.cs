namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SummonAll : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetClients.ToList())
        {
            if (client.GetUser() != null)
            {
                if (client.GetUser().CurrentRoom != null && client.GetUser().CurrentRoom.Id == session.GetUser().CurrentRoom.Id)
                {
                    return;
                }

                client.GetUser().IsTeleporting = true;
                client.GetUser().TeleportingRoomID = room.RoomData.Id;
                client.GetUser().TeleporterId = 0;

                client.SendPacket(new GetGuestRoomResultComposer(client, room.RoomData, false, true));
            }
        }
    }
}
