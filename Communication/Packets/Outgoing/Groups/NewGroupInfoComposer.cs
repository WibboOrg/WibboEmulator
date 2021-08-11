namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class NewGroupInfoComposer : ServerPacket
    {
        public NewGroupInfoComposer(int RoomId, int GroupId)
            : base(ServerPacketHeader.NewGroupInfoMessageComposer)
        {
            this.WriteInteger(RoomId);
            this.WriteInteger(GroupId);
        }
    }
}
