namespace WibboEmulator.Games.Chat.Commands.User.RP;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Steal : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return;
        }

        if (!room.IsRoleplay)
        {
            return;
        }

        var rp = userRoom.Roleplayer;
        if (rp == null)
        {
            return;
        }

        if (rp.Dead || rp.SendPrison)
        {
            return;
        }

        var targetRoomUser = room.GetRoomUserManager().GetRoomUserByName(parameters[1].ToString());

        if (targetRoomUser == null || targetRoomUser.Client == null || targetRoomUser.Client.GetUser() == null)
        {
            return;
        }

        var rpTwo = targetRoomUser.Roleplayer;
        if (rpTwo == null)
        {
            return;
        }

        if (targetRoomUser.Client.GetUser().Id == session.GetUser().Id)
        {
            return;
        }

        if (rpTwo.Dead || rpTwo.SendPrison)
        {
            return;
        }

        var numberMoney = (int)Math.Floor((double)((double)rp.Money / 100) * 15);

        if (rp.Money < numberMoney)
        {
            return;
        }

        if (!(Math.Abs(targetRoomUser.X - userRoom.X) >= 2 || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2))
        {
            rp.Money += numberMoney;
            rpTwo.Money -= numberMoney;

            rp.SendUpdate();
            rpTwo.SendUpdate();

            targetRoomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.vole.receive", targetRoomUser.Client.Langue), numberMoney, userRoom.GetUsername()));

            session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.vole.send", session.Langue), numberMoney, targetRoomUser.GetUsername()));
            userRoom.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.vole.send.chat", session.Langue), targetRoomUser.GetUsername()), 0, true);
        }
    }
}
