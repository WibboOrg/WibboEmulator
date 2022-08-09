using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Groups;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Users;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GiveAdminRightsEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Session.GetUser().Id != Group.CreatorId || !Group.IsMember(UserId))
            {
                return;
            }

            User user = WibboEnvironment.GetUserById(UserId);
            if (user == null)
            {
                return;
            }

            Group.MakeAdmin(UserId);

            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(UserId);
                if (User != null)
                {
                    if (!User.ContainStatus("flatctrl"))
                    {
                        User.SetStatus("flatctrl", "1");
                    }

                    User.UpdateNeeded = true;

                    if (User.GetClient() != null)
                    {
                        User.GetClient().SendPacket(new YouAreControllerComposer(1));
                    }
                }
            }

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, user, 1));
        }
    }
}