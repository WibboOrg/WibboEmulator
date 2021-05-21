using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SetMannequinNameEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string Name = Packet.PopString();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MANNEQUIN)
            {
                return;
            }

            string Look = "";
            foreach (string Part in Session.GetHabbo().Look.Split('.'))
            {
                if (Part.StartsWith("ch") || Part.StartsWith("lg") || Part.StartsWith("cc") || Part.StartsWith("ca") || Part.StartsWith("sh") || Part.StartsWith("wa"))
                {
                    Look = Look + Part + ".";
                }
            }

            Look = Look.Substring(0, Look.Length - 1);
            if (Look.Length > 200)
            {
                Look = Look.Substring(0, 200);
            }

            if (Name.Length > 100)
            {
                Name = Name.Substring(0, 100);
            }

            Name = Name.Replace(";", ":");

            roomItem.ExtraData = Session.GetHabbo().Gender.ToUpper() + ";" + Look + ";" + Name + ";";
            roomItem.UpdateState();
        }
    }
}