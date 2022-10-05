namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Follow : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(Params[1]);

        if (TargetUser == null || TargetUser.GetUser() == null)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", session.Langue));
        }
        else if (TargetUser.GetUser().HideInRoom && !session.GetUser().HasPermission("perm_mod"))
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", session.Langue));
        }
        else if (TargetUser.GetUser().Rank >= 8)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", session.Langue));
        }
        else
        {
            var currentRoom = TargetUser.GetUser().CurrentRoom;
            if (currentRoom != null)
            {
                session.SendPacket(new GetGuestRoomResultComposer(session, currentRoom.RoomData, false, true));
            }
        }
    }
}
