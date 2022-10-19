namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;

internal class LetUserInEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var username = packet.PopString();
        var allowUserToEnter = packet.PopBoolean();

        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(username);
        if (clientByUsername == null || clientByUsername.User == null)
        {
            return;
        }

        if (clientByUsername.User.LoadingRoomId != room.Id)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(clientByUsername.User.Id);

        if (user != null)
        {
            return;
        }

        if (allowUserToEnter)
        {
            clientByUsername.SendPacket(new FlatAccessibleComposer(""));

            clientByUsername.User.AllowDoorBell = true;

            if (!clientByUsername.User.EnterRoom(room))
            {
                clientByUsername.SendPacket(new CloseConnectionComposer());
            }
        }
        else
        {
            clientByUsername.SendPacket(new FlatAccessDeniedComposer(""));
        }
    }
}
