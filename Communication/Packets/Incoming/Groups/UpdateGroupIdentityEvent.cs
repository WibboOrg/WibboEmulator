using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Groups;namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class UpdateGroupIdentityEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            int GroupId = Packet.PopInt();
            string Name = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
            string Desc = ButterflyEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());

            if (Name.Length > 50)
            {
                return;
            }

            if (Desc.Length > 255)
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
                dbClient.SetQuery("UPDATE `groups` SET `name`= @name, `desc` = @desc WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("name", Name);
                dbClient.AddParameter("desc", Desc);
                dbClient.AddParameter("groupId", GroupId);
                dbClient.RunQuery();
            }

            Group.Name = Name;
            Group.Description = Desc;

            Session.SendPacket(new GroupInfoComposer(Group, Session));        }    }}