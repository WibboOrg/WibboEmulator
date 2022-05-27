using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Clients;
using System.Linq;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class SummonAll : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            foreach (Client Client in ButterflyEnvironment.GetGame().GetClientManager().GetClients.ToList())
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
