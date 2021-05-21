using Butterfly.HabboHotel.Groups;

namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class UpdateFavouriteGroupComposer : ServerPacket
    {
        public UpdateFavouriteGroupComposer(Group Group, int VirtualId)
            : base(ServerPacketHeader.UpdateFavouriteGroupMessageComposer)
        {
            this.WriteInteger(VirtualId);//Sends 0 on .COM
            this.WriteInteger(Group != null ? Group.Id : 0);
            this.WriteInteger(3);
            this.WriteString(Group != null ? Group.Name : string.Empty);
        }
    }
}
