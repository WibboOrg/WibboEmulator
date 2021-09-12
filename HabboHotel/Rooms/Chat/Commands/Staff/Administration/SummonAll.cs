using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.HabboHotel.GameClients;using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class SummonAll : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            foreach (GameClient Client in ButterflyEnvironment.GetGame().GetClientManager().GetClients.ToList())            {                if (Client.GetHabbo() != null)                {                    Client.GetHabbo().IsTeleporting = true;                    Client.GetHabbo().TeleportingRoomID = Room.RoomData.Id;                    Client.GetHabbo().TeleporterId = 0;                    Client.SendPacket(new GetGuestRoomResultComposer(Client, Room.RoomData, false, true));                }            }        }    }}