using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class MassBadge : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            string Badge = Params[1];            if (string.IsNullOrEmpty(Badge))
            {
                return;
            }

            foreach (GameClient Client in ButterflyEnvironment.GetGame().GetClientManager().GetClients.ToList())            {                if (Client.GetHabbo() != null)                {                    Client.GetHabbo().GetBadgeComponent().GiveBadge(Badge, true);                    Client.SendPacket(new ReceiveBadgeComposer(Badge));                }            }        }    }}