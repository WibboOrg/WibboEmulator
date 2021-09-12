using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.HabboHotel.GameClients;using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class MassBadge : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            string Badge = Params[1];            if (string.IsNullOrEmpty(Badge))
            {
                return;
            }

            foreach (GameClient Client in ButterflyEnvironment.GetGame().GetClientManager().GetClients.ToList())            {                if (Client.GetHabbo() != null)                {                    Client.GetHabbo().GetBadgeComponent().GiveBadge(Badge, true);                    Client.SendPacket(new ReceiveBadgeComposer(Badge));                }            }        }    }}