namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using System.Text;
using WibboEmulator.Games.Rooms;

internal class UserUpdateComposer : ServerPacket
{
    public UserUpdateComposer(ICollection<RoomUser> RoomUsers)
        : base(ServerPacketHeader.UNIT_STATUS)
    {
        this.WriteInteger(RoomUsers.Count);
        foreach (var User in RoomUsers.ToList())
        {
            this.WriteInteger(User.VirtualId);
            this.WriteInteger(User.X);
            this.WriteInteger(User.Y);
            this.WriteString(User.Z.ToString("0.00"));
            this.WriteInteger(User.RotHead);
            this.WriteInteger(User.RotBody);

            var StatusComposer = new StringBuilder();
            StatusComposer.Append('/');

            foreach (var Status in User.Statusses.ToList())
            {
                StatusComposer.Append(Status.Key);

                if (!string.IsNullOrEmpty(Status.Value))
                {
                    StatusComposer.Append(' ');
                    StatusComposer.Append(Status.Value);
                }

                StatusComposer.Append('/');
            }

            StatusComposer.Append('/');
            this.WriteString(StatusComposer.ToString());
        }
    }
}
