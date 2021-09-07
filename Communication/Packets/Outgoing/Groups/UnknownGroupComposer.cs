﻿namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class UnknownGroupComposer : ServerPacket
    {
        public UnknownGroupComposer(int GroupId, int HabboId)
            : base(ServerPacketHeader.GROUP_MEMBERS_REFRESH)
        {
            this.WriteInteger(GroupId);
            this.WriteInteger(HabboId);
        }
    }
}