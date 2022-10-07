namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Emblem : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (session.GetUser().GetBadgeComponent().HasBadgeSlot("ADM"))
        {
            userRoom.CurrentEffect = 540;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("PRWRD1"))
        {
            userRoom.CurrentEffect = 580;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("GPHWIB"))
        {
            userRoom.CurrentEffect = 557;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur"))
        {
            userRoom.CurrentEffect = 544;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBARC"))
        {
            userRoom.CurrentEffect = 546;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("CRPOFFI"))
        {
            userRoom.CurrentEffect = 570;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("ZEERSWS"))
        {
            userRoom.CurrentEffect = 552;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WBASSO"))
        {
            userRoom.CurrentEffect = 576;
        }
        else if (session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBBOCOM"))
        {
            userRoom.CurrentEffect = 581;
        }

        if (userRoom.CurrentEffect > 0)
        {
            room.SendPacket(new AvatarEffectComposer(userRoom.VirtualId, userRoom.CurrentEffect));
        }
    }
}
