namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class HotelAlert : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var Message = CommandManager.MergeParams(Params, 1);
        if (session.Antipub(Message, "<CMD>", Room.Id))
        {
            return;
        }
        WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new BroadcastMessageAlertComposer(Message + "\r\n" + "- " + session.GetUser().Username));
    }
}