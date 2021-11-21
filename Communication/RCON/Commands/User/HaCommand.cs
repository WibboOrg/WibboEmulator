using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.RCON.Commands.User
{
    internal class HaCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
            {
                return false;
            }

            if (!int.TryParse(parameters[1], out int Userid))
            {
                return false;
            }

            if (Userid == 0)
            {
                return false;
            }

            Client Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
            {
                return false;
            }

            string Message = parameters[2];

            ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", Message));
            if (Client.Antipub(Message, "<alert>"))
            {
                return false;
            }

            ServerPacket message = new ServerPacket(ServerPacketHeader.GENERIC_ALERT);
            message.WriteString(ButterflyEnvironment.GetLanguageManager().TryGetValue("hotelallert.notice", Client.Langue) + "\r\n" + Message + "\r\n- " + Client.GetHabbo().Username);
            ButterflyEnvironment.GetGame().GetClientManager().SendMessage(message);
            return true;
        }
    }
}
