namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SummonAll : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        foreach (var client in GameClientManager.Clients.ToList())
        {
            if (client.User != null)
            {
                if (client.User.Room != null && client.User.Room.Id == Session.User.Room.Id)
                {
                    continue;
                }

                if (client.User.IsTeleporting)
                {
                    continue;
                }

                client.User.IsTeleporting = true;
                client.User.TeleportingRoomID = room.RoomData.Id;
                client.User.TeleporterId = 0;

                client.SendPacket(new GetGuestRoomResultComposer(client, room.RoomData, false, true));
            }
        }
    }
}
