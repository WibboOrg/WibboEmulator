using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class HotelAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            if (Session.Antipub(Message, "<CMD>", Room.Id))
            {
                return;
            }
            ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(Message + "\r\n" + "- " + Session.GetHabbo().Username));
        }
    }
}