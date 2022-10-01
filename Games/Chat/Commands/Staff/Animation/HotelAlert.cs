using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
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
            WibboEnvironment.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(Message + "\r\n" + "- " + Session.GetUser().Username));
        }
    }
}