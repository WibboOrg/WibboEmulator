using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class AddStickyNoteEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
            {
                return;
            }

            int Id = Packet.PopInt();
            string str = Packet.PopString();

            Item userItem = Session.GetUser().GetInventoryComponent().GetItem(Id);
            if (userItem == null)
            {
                return;
            }

            if (room == null)
            {
                return;
            }

            string wallCoord = this.WallPositionCheck(":" + str.Split(':')[1]);
            Item roomItem = new Item(userItem.Id, room.Id, userItem.BaseItem, userItem.ExtraData, userItem.Limited, userItem.LimitedStack, 0, 0, 0.0, 0, wallCoord, room);
            if (!room.GetRoomItemHandler().SetWallItem(Session, roomItem))
            {
                return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.UpdateRoomIdAndUserId(dbClient, Id, room.Id, room.RoomData.OwnerId);
            }

            Session.GetUser().GetInventoryComponent().RemoveItem(Id);
        }

        private string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                { return ":w=0,0 l=0,0 l"; }
                if (wallPosition.Contains(Convert.ToChar(9)))
                { return ":w=0,0 l=0,0 l"; }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                {
                    return ":w=0,0 l=0,0 l";
                }

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                //if (widthX < 0 || widthY < 0 || widthX > 200 || widthY > 200)
                //return ":w=0,0 l=0,0 l";

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                //if (lengthX < 0 || lengthY < 0 || lengthX > 200 || lengthY > 200)
                //return ":w=0,0 l=0,0 l";
                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return ":w=0,0 l=0,0 l";
            }
        }

    }
}
