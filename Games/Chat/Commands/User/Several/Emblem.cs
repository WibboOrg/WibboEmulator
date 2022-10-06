namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Emblem : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (session.GetUser().GetBadgeComponent().HasBadgeSlot("ADM"))
        {
            UserRoom.CurrentEffect = 540;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("PRWRD1"))
        {
            UserRoom.CurrentEffect = 580;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("GPHWIB"))
        {
            UserRoom.CurrentEffect = 557;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur"))
        {
            UserRoom.CurrentEffect = 544;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBARC"))
        {
            UserRoom.CurrentEffect = 546;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("CRPOFFI"))
        {
            UserRoom.CurrentEffect = 570;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("ZEERSWS"))
        {
            UserRoom.CurrentEffect = 552;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WBASSO"))
        {
            UserRoom.CurrentEffect = 576;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBBOCOM"))
        {
            UserRoom.CurrentEffect = 581;
        }

        if (UserRoom.CurrentEffect > 0)
        {
            Room.SendPacket(new AvatarEffectComposer(UserRoom.VirtualId, UserRoom.CurrentEffect));
        }
    }
}
