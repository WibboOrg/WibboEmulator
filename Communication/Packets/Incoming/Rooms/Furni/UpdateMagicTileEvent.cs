namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class UpdateMagicTileEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session != null && Session.User != null)
        {
            var itemId = packet.PopInt();
            var heightToSet = packet.PopInt();

            if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
            {
                return;
            }

            if (!room.CheckRights(Session))
            {
                return;
            }

            var item = room.RoomItemHandling.GetItem(itemId);
            if (item != null && item.ItemData.InteractionType == InteractionType.PILE_MAGIC)
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
