using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.HabboHotel.GameClients;
using System.Threading;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Invisible : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().SpectatorMode)
            {
                Session.GetHabbo().SpectatorMode = false;
                Session.GetHabbo().HideInRoom = false;

                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("invisible.disabled", Session.Langue));
            }
            else
            {
                Session.GetHabbo().SpectatorMode = true;
                Session.GetHabbo().HideInRoom = true;

                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("invisible.enabled", Session.Langue));

            }

            //UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("invisible.waiting", Session.Langue));

            Session.SendPacket(new GetGuestRoomResultComposer(Session, Room.RoomData, false, true));
        }
    }
}
