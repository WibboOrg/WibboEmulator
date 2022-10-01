using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SetMannequinFigureEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
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

            string[] Stuff = roomItem.ExtraData.Split(new char[1] { ';' });
            string Name = "";

            if (Stuff.Length >= 3)
            {
                Name = Stuff[2];
            }

            roomItem.ExtraData = Session.GetUser().Gender.ToUpper() + ";" + Look + ";" + Name;
            roomItem.UpdateState();
        }
    }
}
