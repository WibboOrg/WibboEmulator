using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Rooms;
using System.Linq;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateGroupSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Guild Group))
            {
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id)
            {
                return;
            }

            int Type = Packet.PopInt();
            int FurniOptions = Packet.PopInt();

            switch (Type)
            {
                default:
                case 0:
                    Group.GroupType = GuildType.OPEN;
                    break;
                case 1:
                    Group.GroupType = GuildType.LOCKED;
                    break;
                case 2:
                    Group.GroupType = GuildType.PRIVATE;
                    break;
            }

            if (Group.GroupType != GuildType.LOCKED)
            {
                if (Group.GetRequests.Count > 0)
                {
                    foreach (int UserId in Group.GetRequests.ToList())
                    {
                        Group.HandleRequest(UserId, false);
                    }

                    Group.ClearRequests();
                }
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                GuildDao.UpdateStateAndDeco(dbClient, Group.Id, (Group.GroupType == GuildType.OPEN ? 0 : Group.GroupType == GuildType.LOCKED ? 1 : 2), FurniOptions);
            }

            Group.AdminOnlyDeco = FurniOptions;

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
            {
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (Room.RoomData.OwnerId == User.UserId || Group.IsAdmin(User.UserId) || !Group.IsMember(User.UserId))
                {
                    continue;
                }

                if (FurniOptions == 1)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreControllerComposer(0));
                }
                else if (FurniOptions == 0 && !User.Statusses.ContainsKey("flatctrl 1"))
                {
                    User.SetStatus("flatctrl 1", "");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreControllerComposer(1));
                }
            }

            Session.SendPacket(new GroupInfoComposer(Group, Session));
        }
    }
}
