using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class SummonAll : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            foreach (GameClient Client in WibboEnvironment.GetGame().GetGameClientManager().GetClients.ToList())
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
