using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GiveAdminRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Session.GetHabbo().Id != Group.CreatorId || !Group.IsMember(UserId))
            {
                return;
            }

            Habbo Habbo = ButterflyEnvironment.GetHabboById(UserId);
            if (Habbo == null)
            {
                return;
            }

            Group.MakeAdmin(UserId);

            if (ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(UserId);
                if (User != null)
                {
                    if (!User.Statusses.ContainsKey("flatctrl 3"))
                    {
                        User.SetStatus("flatctrl 3", "");
                    }

                    User.UpdateNeeded = true;

                    if (User.GetClient() != null)
                    {
                        User.GetClient().SendPacket(new YouAreControllerComposer(3));
                    }
                }
            }

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, Habbo, 1));
        }
    }
}