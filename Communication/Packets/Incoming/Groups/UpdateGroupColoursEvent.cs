using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Items;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateGroupColoursEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int Colour1 = Packet.PopInt();
            int Colour2 = Packet.PopInt();

            if (Colour1 < 0 || Colour1 > 200)
            {
                return;
            }

            if (Colour2 < 0 || Colour2 > 200)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `colour1` = @colour1, `colour2` = @colour2 WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("colour1", Colour1);
                dbClient.AddParameter("colour2", Colour2);
                dbClient.AddParameter("groupId", Group.Id);
                dbClient.RunQuery();
            }

            Group.Colour1 = Colour1;
            Group.Colour2 = Colour2;

            Session.SendPacket(new GroupInfoComposer(Group, Session));
            if (Session.GetHabbo().CurrentRoom != null)
            {
                foreach (Item Item in Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetFloor.ToList())
                {
                    if (Item == null || Item.GetBaseItem() == null)
                    {
                        continue;
                    }

                    if (Item.GetBaseItem().InteractionType != InteractionType.GUILD_ITEM && Item.GetBaseItem().InteractionType != InteractionType.GUILD_GATE)
                    {
                        continue;
                    }

                    Session.GetHabbo().CurrentRoom.SendPacket(new ObjectUpdateComposer(Item, Session.GetHabbo().CurrentRoom.RoomData.OwnerId));
                }
            }
        }
    }
}
