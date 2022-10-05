namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Summon : IChatCommand
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
            return;
        }
        else if (TargetUser.GetUser().CurrentRoom != null && TargetUser.GetUser().CurrentRoom.Id == session.GetUser().CurrentRoom.Id)
        {
            return;
        }

        var currentRoom = session.GetUser().CurrentRoom;
        TargetUser.GetUser().IsTeleporting = true;
        TargetUser.GetUser().TeleportingRoomID = currentRoom.RoomData.Id;
        TargetUser.GetUser().TeleporterId = 0;

        TargetUser.SendPacket(new GetGuestRoomResultComposer(TargetUser, currentRoom.RoomData, false, true));
    }
}