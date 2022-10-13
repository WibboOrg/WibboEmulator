namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class UpdateMagicTileEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session != null && session.GetUser() != null)
        {
            var itemId = packet.PopInt();
            var heightToSet = packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
            {
                return;
            }

            if (!room.CheckRights(session))
            {
                return;
            }

            var item = room.RoomItemHandling.GetItem(itemId);
            if (item != null && item.GetBaseItem().InteractionType == InteractionType.PILEMAGIC)
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

                room.SendPacket(new ObjectUpdateComposer(item, room.RoomData.OwnerId));
            }
        }
    }
}
