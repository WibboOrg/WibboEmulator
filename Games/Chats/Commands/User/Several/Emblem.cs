namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Emblem : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.User.BadgeComponent.HasBadgeSlot("ADM"))
        {
            userRoom.CurrentEffect = 540;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("PRWRD1"))
        {
            userRoom.CurrentEffect = 580;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("GPHWIB"))
        {
            userRoom.CurrentEffect = 557;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("wibbo.helpeur"))
        {
            userRoom.CurrentEffect = 544;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("WIBARC"))
        {
            userRoom.CurrentEffect = 546;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("CRPOFFI"))
        {
            userRoom.CurrentEffect = 570;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("ZEERSWS"))
        {
            userRoom.CurrentEffect = 552;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("WBASSO"))
        {
            userRoom.CurrentEffect = 576;
        }
        else if (session.User.BadgeComponent.HasBadgeSlot("WIBBOCOM"))
        {
            userRoom.CurrentEffect = 581;
        }

        if (userRoom.CurrentEffect > 0)
        {
            room.SendPacket(new AvatarEffectComposer(userRoom.VirtualId, userRoom.CurrentEffect));
        }
    }
}
