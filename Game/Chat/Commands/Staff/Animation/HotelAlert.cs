using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class HotelAlert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
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