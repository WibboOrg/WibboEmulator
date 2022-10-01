using WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Emblem : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("ADM"))
            {
                UserRoom.CurrentEffect = 540;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("PRWRD1"))
            {
                UserRoom.CurrentEffect = 580;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("GPHWIB"))
            {
                UserRoom.CurrentEffect = 557;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur"))
            {
                UserRoom.CurrentEffect = 544;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBARC"))
            {
                UserRoom.CurrentEffect = 546;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("CRPOFFI"))
            {
                UserRoom.CurrentEffect = 570;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("ZEERSWS"))
            {
                UserRoom.CurrentEffect = 552;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("WBASSO"))
            {
                UserRoom.CurrentEffect = 576;
            }
            else if (Session.GetUser().GetBadgeComponent().HasBadgeSlot("WIBBOCOM"))
            {
                UserRoom.CurrentEffect = 581;
            }

            if (UserRoom.CurrentEffect > 0)
            {
                Room.SendPacket(new AvatarEffectComposer(UserRoom.VirtualId, UserRoom.CurrentEffect));
            }
        }
    }
}
