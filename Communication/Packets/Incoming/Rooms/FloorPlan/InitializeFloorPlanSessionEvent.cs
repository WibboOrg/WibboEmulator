namespace WibboEmulator.Communication.Packets.Incoming.Rooms.FloorPlan;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.FloorPlan;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class InitializeFloorPlanSessionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        Session.SendPacket(new FloorPlanSendDoorComposer(room.GameMap.Model.DoorX, room.GameMap.Model.DoorY, room.GameMap.Model.DoorOrientation));
    }
}
