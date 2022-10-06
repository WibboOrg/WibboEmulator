namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;

internal class MoveWallItemEvent : IPacketEvent
{
    public double Delay => 200;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var Id = packet.PopInt();
        var str = packet.PopString();

        var roomItem = room.GetRoomItemHandler().GetItem(Id);
        if (roomItem == null)
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", session.Langue));
            return;
        }

        var wallCoordinate = WallPositionCheck(":" + str.Split(':')[1]);
        roomItem.WallCoord = wallCoordinate;
        room.GetRoomItemHandler().UpdateItem(roomItem);

        room.SendPacket(new ItemUpdateComposer(roomItem, room.RoomData.OwnerId));

    }

    private static string WallPositionCheck(string wallPosition)
    {
        //:w=3,2 l=9,63 l
        try
        {
            if (wallPosition.Contains(Convert.ToChar(13)))
            { return ":w=0,0 l=0,0 l"; }
            if (wallPosition.Contains(Convert.ToChar(9)))
            { return ":w=0,0 l=0,0 l"; }

            var posD = wallPosition.Split(' ');
            if (posD[2] is not "l" and not "r")
            {
                return ":w=0,0 l=0,0 l";
            }

            var widD = posD[0][3..].Split(',');
            var widthX = int.Parse(widD[0]);
            var widthY = int.Parse(widD[1]);
            //if (widthX < 0 || widthY < 0 || widthX > 200 || widthY > 200)
            //return ":w=0,0 l=0,0 l";

            var lenD = posD[1][2..].Split(',');
            var lengthX = int.Parse(lenD[0]);
            var lengthY = int.Parse(lenD[1]);
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
