using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class RoomBadge : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }

            string local_0 = Params[1];
            foreach (RoomUser item_0 in currentRoom.GetRoomUserManager().GetUserList().ToList())
            {
                try
                {
                    if (!item_0.IsBot)
                    {
                        if (item_0.GetClient() != null)
                        {
                            if (item_0.GetClient().GetUser() != null)
                            {
                                item_0.GetClient().GetUser().GetBadgeComponent().GiveBadge(local_0, true);
                                item_0.GetClient().SendPacket(new ReceiveBadgeComposer(local_0));
                            }
                        }
                    }
                }
                catch
                {
                }
            }

        }
    }
}