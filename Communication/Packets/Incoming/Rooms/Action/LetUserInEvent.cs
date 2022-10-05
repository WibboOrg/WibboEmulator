namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Games.GameClients;

internal class LetUserInEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session))
        {
            return;
        }

        var username = Packet.PopString();
        var allowUserToEnter = Packet.PopBoolean();

        var clientByUsername = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(username);
        if (clientByUsername == null || clientByUsername.GetUser() == null)
        {
            return;
        }

        if (clientByUsername.GetUser().LoadingRoomId != room.Id)
        {
            return;
        }

        var user = room.GetRoomUserManager().GetRoomUserByUserId(clientByUsername.GetUser().Id);

        if (user != null)
        {
            return;
        }

        if (allowUserToEnter)
        {
            clientByUsername.SendPacket(new FlatAccessibleComposer(""));

            clientByUsername.GetUser().AllowDoorBell = true;

            if (!clientByUsername.GetUser().EnterRoom(session.GetUser().CurrentRoom))
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