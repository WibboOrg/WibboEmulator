using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class SummonAll : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            foreach (Client Client in WibboEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client.GetUser() != null)
                {
                    if (Client.GetUser().CurrentRoom != null && Client.GetUser().CurrentRoom.Id == Session.GetUser().CurrentRoom.Id)
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
}
