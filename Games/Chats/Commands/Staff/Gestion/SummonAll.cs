namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SummonAll : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        foreach (var client in WibboEnvironment.GetGame().GetGameClientManager().GetClients.ToList())
        {
            if (client.User != null)
            {
                if (client.User.CurrentRoom != null && client.User.CurrentRoom.Id == session.User.CurrentRoom.Id)
                {
                    return;
                }

                client.User.IsTeleporting = true;
                client.User.TeleportingRoomID = room.RoomData.Id;
                client.User.TeleporterId = 0;

                client.SendPacket(new GetGuestRoomResultComposer(client, room.RoomData, false, true));
            }
        }
    }
}
