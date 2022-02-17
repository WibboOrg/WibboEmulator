using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateMagicTileEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session != null && Session.GetHabbo() != null)
            {
                int ItemId = Packet.PopInt();
                int HeightToSet = Packet.PopInt();
                Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
                if ((room == null ? false : room.CheckRights(Session)))
                {
                    Item item = room.GetRoomItemHandler().GetItem(ItemId);
                    if ((item == null ? false : item.GetBaseItem().InteractionType == InteractionType.PILEMAGIC))
                    {
                        if (HeightToSet > 5000)
                        {
                            HeightToSet = 5000;
                        }
                        if (HeightToSet < 0)
                        {
                            HeightToSet = 0;
                        }

                        double TotalZ = (double)(HeightToSet / 100.00);

                        item.SetState(item.X, item.Y, TotalZ, item.GetAffectedTiles);

                        room.SendPacket(new ObjectUpdateComposer(item, room.RoomData.OwnerId));
                    }
                }
            }
        }
    }
}
