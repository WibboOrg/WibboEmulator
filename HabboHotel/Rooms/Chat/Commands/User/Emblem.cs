using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Emblem : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ADM"))
            {
                UserRoom.CurrentEffect = 540;
            }
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PRWRD1"))
            {
                UserRoom.CurrentEffect = 580;
            }
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("GPHWIB"))
            {
                UserRoom.CurrentEffect = 557;
            }
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("wibbo.helpeur"))
            {
                UserRoom.CurrentEffect = 544;
            }
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("WIBARC"))
            {
                UserRoom.CurrentEffect = 546;
            }
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("CRPOFFI"))
            {
                UserRoom.CurrentEffect = 570;
            }
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ZEERSWS"))
            {
                UserRoom.CurrentEffect = 552;
            }
            else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("WBASSO"))
            {
                UserRoom.CurrentEffect = 576;
            }
            //else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("BALTIOFFI"))
            //UserRoom.CurrentEffect = 578;
            //else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("WFMEM"))
            //UserRoom.CurrentEffect = 579;

            if (UserRoom.CurrentEffect > 0)
            {
                Room.SendPacket(new AvatarEffectComposer(UserRoom.VirtualId, UserRoom.CurrentEffect));
            }
        }
    }
}
