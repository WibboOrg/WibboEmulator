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
            var ItemId = packet.PopInt();
            var HeightToSet = packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
            {
                return;
            }

            if (!room.CheckRights(session))
            {
                return;
            }

            var item = room.GetRoomItemHandler().GetItem(ItemId);
            if (item == null ? false : item.GetBaseItem().InteractionType == InteractionType.PILEMAGIC)
            {
                if (HeightToSet > 5000)
                {
                    HeightToSet = 5000;
                }
                if (HeightToSet < 0)
                {
                    HeightToSet = 0;
                }

                var TotalZ = (double)(HeightToSet / 100.00);

                item.SetState(item.X, item.Y, TotalZ, item.GetAffectedTiles);

                room.SendPacket(new ObjectUpdateComposer(item, room.RoomData.OwnerId));
            }
        }
    }
}
