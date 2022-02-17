using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Game.Clients;using Butterfly.Game.Guilds;using Butterfly.Game.Rooms;
using Butterfly.Game.Users;namespace Butterfly.Communication.Packets.Incoming.Structure{    internal class TakeAdminRightsEvent : IPacketEvent    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)        {            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            if (Session.GetHabbo().Id != Group.CreatorId || !Group.IsMember(UserId))
            {
                return;
            }

            User Habbo = ButterflyEnvironment.GetHabboById(UserId);
            if (Habbo == null)
            {
                return;
            }

            Group.TakeAdmin(UserId);

            if (ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room Room))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(UserId);
                if (User != null)
                {
                    if (User.Statusses.ContainsKey("flatctrl 3"))
                    {
                        User.RemoveStatus("flatctrl 3");
                    }

                    User.UpdateNeeded = true;
                    if (User.GetClient() != null)
                    {
                        User.GetClient().SendPacket(new YouAreControllerComposer(0));
                    }
                }
            }

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, Habbo, 2));        }    }}