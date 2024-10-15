namespace WibboEmulator.Games.Chats.Commands.User.RP;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class GiveMoney : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
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

        var targetRoomUser = room.RoomUserManager.GetRoomUserByName(parameters[1].ToString());

        if (targetRoomUser == null || targetRoomUser.Client == null || targetRoomUser.Client.User == null)
        {
            return;
        }

        if (!int.TryParse(parameters[2].ToString(), out var numberMoney))
        {
            return;
        }

        if (numberMoney <= 0)
        {
            return;
        }

        var rpTwo = targetRoomUser.Roleplayer;
        if (rpTwo == null)
        {
            return;
        }

        if (targetRoomUser.Client.User.Id == Session.User.Id)
        {
            return;
        }

        if (rpTwo.Dead || rpTwo.SendPrison)
        {
            return;
        }

        if (rp.Money < numberMoney)
        {
            return;
        }

        if (!(Math.Abs(targetRoomUser.X - userRoom.X) >= 2 || Math.Abs(targetRoomUser.Y - userRoom.Y) >= 2))
        {
            rp.Money -= numberMoney;
            rpTwo.Money += numberMoney;

            rp.SendUpdate();
            rpTwo.SendUpdate();

            targetRoomUser.SendWhisperChat(string.Format(LanguageManager.TryGetValue("rp.givemoney.receive", targetRoomUser.Client.Language), numberMoney, userRoom.Username));

            Session.SendWhisper(string.Format(LanguageManager.TryGetValue("rp.givemoney.send", Session.Language), numberMoney, targetRoomUser.Username));
            userRoom.OnChat(string.Format(LanguageManager.TryGetValue("rp.givemoney.send.chat", Session.Language), targetRoomUser.Username), 0, true);
        }
    }
}
