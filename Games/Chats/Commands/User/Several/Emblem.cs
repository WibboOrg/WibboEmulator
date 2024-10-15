namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Emblem : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var emblemId = Session.User.BadgeComponent.EmblemId;

        if (emblemId > 0)
        {
            userRoom.CurrentEffect = emblemId;
            room.SendPacket(new AvatarEffectComposer(userRoom.VirtualId, userRoom.CurrentEffect));
        }
    }
}
