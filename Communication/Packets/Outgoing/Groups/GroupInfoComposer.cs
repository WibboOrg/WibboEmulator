using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using System;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class GroupInfoComposer : ServerPacket
    {
        public GroupInfoComposer(Group Group, GameClient Session, bool NewWindow = false)
            : base(ServerPacketHeader.GROUP_INFO)
        {
            DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Group.CreateTime);

            this.WriteInteger(Group.Id);
            this.WriteBoolean(true);
            this.WriteInteger(Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2);
            this.WriteString(Group.Name);
            this.WriteString(Group.Description);
            this.WriteString(Group.Badge);
            this.WriteInteger(Group.RoomId);
            this.WriteString((ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId) == null) ? "No room found.." : ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(Group.RoomId).Name);    // room name
            this.WriteInteger(Group.CreatorId == Session.GetHabbo().Id ? 3 : Group.HasRequest(Session.GetHabbo().Id) ? 2 : Group.IsMember(Session.GetHabbo().Id) ? 1 : 0);
            this.WriteInteger(Group.MemberCount); // Members
            this.WriteBoolean(false);//?? CHANGED
            this.WriteString(Origin.Day + "-" + Origin.Month + "-" + Origin.Year);
            this.WriteBoolean(Group.CreatorId == Session.GetHabbo().Id);
            this.WriteBoolean(Group.IsAdmin(Session.GetHabbo().Id)); // admin
            this.WriteString(ButterflyEnvironment.GetUsernameById(Group.CreatorId));
            this.WriteBoolean(NewWindow); // Show group info
            this.WriteBoolean(Group.AdminOnlyDeco == 0); // Any user can place furni in home room
            this.WriteInteger(Group.CreatorId == Session.GetHabbo().Id ? Group.RequestCount : Group.IsAdmin(Session.GetHabbo().Id) ? Group.RequestCount : Group.IsMember(Session.GetHabbo().Id) ? 0 : 0); // Pending users
            //base.WriteInteger(0);//what the fuck
            this.WriteBoolean(Group != null ? Group.ForumEnabled : true);//HabboTalk.
        }
    }
}