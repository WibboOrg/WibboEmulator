using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class HotelAlert : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            if (Session.Antipub(Message, "<CMD>", Room.Id))
            {
                return;
            }

            //ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("staff", "Message des Staffs", Message, "Compris !", 0, ""), Session.Langue);

            ServerPacket alert = new ServerPacket(ServerPacketHeader.GENERIC_ALERT);
            alert.WriteString(ButterflyEnvironment.GetLanguageManager().TryGetValue("hotelallert.notice", Session.Langue) + "\r\n" + Message + "\r\n- " + Session.GetHabbo().Username);
            ButterflyEnvironment.GetGame().GetClientManager().SendMessage(alert);

        }
    }
}
