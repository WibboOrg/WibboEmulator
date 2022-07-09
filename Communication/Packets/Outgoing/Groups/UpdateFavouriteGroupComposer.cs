using Wibbo.Game.Groups;

namespace Wibbo.Communication.Packets.Outgoing.Groups
{
    internal class UpdateFavouriteGroupComposer : ServerPacket
    {
        public UpdateFavouriteGroupComposer(Group Group, int VirtualId)
            : base(ServerPacketHeader.FAVORITE_GROUP_UDPATE)
        {
            this.WriteInteger(VirtualId);//Sends 0 on .COM
            this.WriteInteger(Group != null ? Group.Id : 0);
            this.WriteInteger(3);
            this.WriteString(Group != null ? Group.Name : string.Empty);
        }
    }
}
