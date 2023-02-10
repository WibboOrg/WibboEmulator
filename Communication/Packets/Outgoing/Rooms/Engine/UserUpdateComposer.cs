namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using System.Text;
using WibboEmulator.Games.Rooms;

internal sealed class UserUpdateComposer : ServerPacket
{
    public UserUpdateComposer(ICollection<RoomUser> roomUsers)
        : base(ServerPacketHeader.UNIT_STATUS)
    {
        this.WriteInteger(roomUsers.Count);
        foreach (var user in roomUsers.ToList())
        {
            this.WriteInteger(user.VirtualId);
            this.WriteInteger(user.X);
            this.WriteInteger(user.Y);
            this.WriteString(user.Z.ToString("0.00"));
            this.WriteInteger(user.RotHead);
            this.WriteInteger(user.RotBody);

            var statusComposer = new StringBuilder();
            _ = statusComposer.Append('/');

            foreach (var status in user.Statusses.ToList())
            {
                _ = statusComposer.Append(status.Key);

                if (!string.IsNullOrEmpty(status.Value))
                {
                    _ = statusComposer.Append(' ');
                    _ = statusComposer.Append(status.Value);
                }

                _ = statusComposer.Append('/');
            }

            _ = statusComposer.Append('/');
            this.WriteString(statusComposer.ToString());
        }
    }
}
