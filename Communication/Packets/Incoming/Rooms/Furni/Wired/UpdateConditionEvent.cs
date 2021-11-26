using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Items.Wired;
using Butterfly.Game.Rooms;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateConditionEvent : IPacketEvent
    {
        public void Parse(Client session, ClientPacket packet)
        {
            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return;
            }

            if (!room.CheckRights(session) && !room.CheckRights(session, true))
            {
                return;
            }

            int itemId = packet.PopInt();

            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
            {
                return;
            }

            List<int> intParams = new List<int>();
            int countInt = packet.PopInt();
            for (int i = 0; i < countInt; i++)
            {
                intParams.Add(packet.PopInt());
            }

            string stringParam = packet.PopString();

            List<int> stuffIds = new List<int>();
            int countStuff = packet.PopInt();
            for (int i = 0; i < countStuff; i++)
            {
                stuffIds.Add(packet.PopInt());
            }

            int selectionCode = packet.PopInt();

            bool isStaff = session.GetHabbo().HasFuse("fuse_superwired_staff");
            bool isGod = session.GetHabbo().HasFuse("fuse_superwired_god");

            WiredRegister.HandleRegister(item, room, intParams, stringParam, stuffIds, selectionCode, 0, isStaff, isGod);

            session.SendPacket(new SaveWiredComposer());
        }
    }
}
