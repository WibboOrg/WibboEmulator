using Wibbo.Game.Users;

namespace Wibbo.Communication.Packets.Outgoing.Handshake
{
    internal class UserObjectComposer : ServerPacket
    {
        public UserObjectComposer(User user)
            : base(ServerPacketHeader.USER_INFO)
        {
            this.WriteInteger(user.Id);
            this.WriteString(user.Username);
            this.WriteString(user.Look);
            this.WriteString(user.Gender.ToUpper());
            this.WriteString(user.Motto);
            this.WriteString("");
            this.WriteBoolean(false);
            this.WriteInteger(user.Respect);
            this.WriteInteger(user.DailyRespectPoints);
            this.WriteInteger(user.DailyPetRespectPoints);
            this.WriteBoolean(false); // Friends stream active
            this.WriteString(user.LastOnline.ToString()); // last online?
            this.WriteBoolean(user.Rank > 1 || user.CanChangeName); // Can change name
            this.WriteBoolean(false);
        }
    }
}
