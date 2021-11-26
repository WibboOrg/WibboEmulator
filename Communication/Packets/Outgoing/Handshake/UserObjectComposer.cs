using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Outgoing.Handshake
{
    internal class UserObjectComposer : ServerPacket
    {
        public UserObjectComposer(User Habbo)
            : base(ServerPacketHeader.USER_INFO)
        {
            this.WriteInteger(Habbo.Id);
            this.WriteString(Habbo.Username);
            this.WriteString(Habbo.Look);
            this.WriteString(Habbo.Gender.ToUpper());
            this.WriteString(Habbo.Motto);
            this.WriteString("");
            this.WriteBoolean(false);
            this.WriteInteger(Habbo.Respect);
            this.WriteInteger(Habbo.DailyRespectPoints);
            this.WriteInteger(Habbo.DailyPetRespectPoints);
            this.WriteBoolean(false); // Friends stream active
            this.WriteString(Habbo.LastOnline.ToString()); // last online?
            this.WriteBoolean(Habbo.Rank > 1 || Habbo.CanChangeName); // Can change name
            this.WriteBoolean(false);
        }
    }
}
