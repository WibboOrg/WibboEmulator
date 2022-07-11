using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Groups;
using WibboEmulator.Game.Items;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class UpdateGroupColoursEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
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

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetUser().Id)
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildDao.UpdateColors(dbClient, Colour1, Colour2, Group.Id);
            }

            Group.Colour1 = Colour1;
            Group.Colour2 = Colour2;

            Session.SendPacket(new GroupInfoComposer(Group, Session));
            if (Session.GetUser().CurrentRoom != null)
            {
                foreach (Item Item in Session.GetUser().CurrentRoom.GetRoomItemHandler().GetFloor.ToList())
                {
                    if (Item == null || Item.GetBaseItem() == null)
                    {
                        continue;
                    }

                    if (Item.GetBaseItem().InteractionType != InteractionType.GUILD_ITEM && Item.GetBaseItem().InteractionType != InteractionType.GUILD_GATE)
                    {
                        continue;
                    }

                    Session.GetUser().CurrentRoom.SendPacket(new ObjectUpdateComposer(Item, Session.GetUser().CurrentRoom.RoomData.OwnerId));
                }
            }
        }
    }
}
