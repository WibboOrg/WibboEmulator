using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class SetMannequinNameEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string Name = Packet.PopString();

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
            foreach (string Part in Session.GetUser().Look.Split('.'))
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

            roomItem.ExtraData = Session.GetUser().Gender.ToUpper() + ";" + Look + ";" + Name;
            roomItem.UpdateState();
        }
    }
}