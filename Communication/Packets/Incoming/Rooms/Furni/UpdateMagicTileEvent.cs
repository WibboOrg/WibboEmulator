namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class UpdateMagicTileEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session != null && session.User != null)
        {
            var itemId = packet.PopInt();
            var heightToSet = packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
            {
                return;
            }

            if (!room.CheckRights(session))
            {
                return;
            }

            var item = room.RoomItemHandling.GetItem(itemId);
            if (item != null && item.GetBaseItem().InteractionType == InteractionType.PILE_MAGIC)
            {
                if (heightToSet > 5000)
                {
                    heightToSet = 5000;
                }
                if (heightToSet < 0)
                {
                    heightToSet = 0;
                }

                var totalZ = (double)(heightToSet / 100.00);

                item.SetState(item.X, item.Y, totalZ);

                item.UpdateState(false);
            }
        }
    }
}
