namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class SummonAll : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        foreach (var Client in WibboEnvironment.GetGame().GetGameClientManager().GetClients.ToList())
        {
            if (Client.GetUser() != null)
            {
                if (Client.GetUser().CurrentRoom != null && Client.GetUser().CurrentRoom.Id == session.GetUser().CurrentRoom.Id)
                {
                    return;
                }

                Client.GetUser().IsTeleporting = true;
                Client.GetUser().TeleportingRoomID = Room.RoomData.Id;
                Client.GetUser().TeleporterId = 0;

                Client.SendPacket(new GetGuestRoomResultComposer(Client, Room.RoomData, false, true));
            }
        }
    }
}
