namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class UserIsStaffComposer : ServerPacket
    {
        public UserIsStaffComposer(bool IsStaff)
            : base(2)
        {
            this.WriteBoolean(IsStaff);
        }
    }
}
