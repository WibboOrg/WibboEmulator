namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomBadge : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var currentRoom = session.GetUser().CurrentRoom;
        if (currentRoom == null)
        {
            return;
        }

        var local_0 = parameters[1];
        foreach (var item_0 in currentRoom.GetRoomUserManager().GetUserList().ToList())
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
