namespace WibboEmulator.Communication.Packets.Incoming.Inventory.AvatarEffects;

using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class AvatarEffectSelectedEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var effectId = packet.PopInt();

        if (effectId < 0)
        {
            return;
        }

        if (!EffectManager.HasEffect(effectId, Session.User.HasPermission("god")))
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);

        if (user == null)
        {
            return;
        }

        var currentEnable = user.CurrentEffect;
        if (currentEnable is 28 or 29 or 30 or 37 or 184 or 77 or 103
            or 40 or 41 or 42 or 43
            or 49 or 50 or 51 or 52
            or 33 or 34 or 35 or 36)
        {
            return;
        }

        if (user.Team != TeamType.None || user.InGame || room.IsGameMode)
        {
            return;
        }

        user.ApplyEffect(effectId);
    }
}
