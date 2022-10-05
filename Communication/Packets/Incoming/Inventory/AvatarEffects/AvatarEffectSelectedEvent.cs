namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games;

internal class AvatarEffectSelectedEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var NumEnable = Packet.PopInt();

        if (NumEnable < 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, session.GetUser().HasPermission("perm_god")))
        {
            return;
        }

        var Room = session.GetUser().CurrentRoom;
        if (Room == null)
        {
            return;
        }

        var User = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);

        if (User == null)
        {
            return;
        }

        var CurrentEnable = User.CurrentEffect;
        if (CurrentEnable is 28 or 29 or 30 or 37 or 184 or 77 or 103
            or 40 or 41 or 42 or 43
            or 49 or 50 or 51 or 52
            or 33 or 34 or 35 or 36)
        {
            return;
        }

        if (User.Team != TeamType.NONE || User.InGame)
        {
            return;
        }

        User.ApplyEffect(NumEnable);
    }
}