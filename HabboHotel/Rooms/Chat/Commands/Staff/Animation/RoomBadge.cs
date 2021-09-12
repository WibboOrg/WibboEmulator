using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.HabboHotel.GameClients;using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class RoomBadge : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            Room currentRoom = Session.GetHabbo().CurrentRoom;            if (currentRoom == null)
            {
                return;
            }

            string local_0 = Params[1];            foreach (RoomUser item_0 in currentRoom.GetRoomUserManager().GetUserList().ToList())            {                try                {                    if (!item_0.IsBot)                    {                        if (item_0.GetClient() != null)                        {                            if (item_0.GetClient().GetHabbo() != null)                            {                                item_0.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(local_0, true);                                item_0.GetClient().SendPacket(new ReceiveBadgeComposer(local_0));                            }                        }                    }                }                catch                {                }            }        }    }}