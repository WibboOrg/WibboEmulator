using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.GameClients;
using System.Linq;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class RoomBadge : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
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
                            if (item_0.GetClient().GetHabbo() != null)
                            {
                                item_0.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(local_0, true);
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
