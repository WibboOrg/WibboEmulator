namespace WibboEmulator.Communication.Packets.Incoming.Rooms.FloorPlan;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.FloorPlan;
using WibboEmulator.Games.GameClients;

internal class GetOccupiedTilesEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        session.SendPacket(new FloorPlanFloorMapComposer(room.GetGameMap().CoordinatedItems));
    }
}
