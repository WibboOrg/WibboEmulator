using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CheckValidNameEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null || Session == null)
            {
                return;
            }

            string Name = Packet.PopString();

            ServerPacket Response = new ServerPacket(ServerPacketHeader.NameChangeUpdateMessageComposer);
            switch (this.NameAvailable(Name))
            {
                case -2:
                    Response.WriteInteger(4);
                    Response.WriteString(Name);
                    Response.WriteInteger(0);
                    break;
                case -1:
                    Response.WriteInteger(4);
                    Response.WriteString(Name);
                    Response.WriteInteger(0);
                    break;
                case 0:
                    Response.WriteInteger(5);
                    Response.WriteString(Name);
                    Response.WriteInteger(2);
                    Response.WriteString("--" + Name + "--");
                    Response.WriteString("Xx" + Name + "xX");
                    break;
                default:
                    Response.WriteInteger(0);
                    Response.WriteString(Name);
                    Response.WriteInteger(0);
                    break;
            }

            Session.SendPacket(Response);
        }

        private int NameAvailable(string Username)
        {
            Username = Username.ToLower();

            if (Username.Length > 15)
            {
                return -2;
            }

            if (Username.Length < 3)
            {
                return -2;
            }

            if (!ButterflyEnvironment.IsValidAlphaNumeric(Username))
            {
                return -1;
            }

            return ButterflyEnvironment.UsernameExists(Username) ? 0 : 1;
        }
    }
}