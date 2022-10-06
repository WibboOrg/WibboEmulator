namespace WibboEmulator.Games.Chat.Commands.User.RP;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GiveMoney : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return;
        }

        if (!Room.IsRoleplay)
        {
            return;
        }

        var Rp = UserRoom.Roleplayer;
        if (Rp == null)
        {
            return;
        }

        if (Rp.Dead || Rp.SendPrison)
        {
            return;
        }

        var TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(parameters[1].ToString());

        if (TargetRoomUser == null || TargetRoomUser.GetClient() == null || TargetRoomUser.GetClient().GetUser() == null)
        {
            return;
        }

        if (!int.TryParse(parameters[2].ToString(), out var NumberMoney))
        {
            return;
        }

        if (NumberMoney <= 0)
        {
            return;
        }

        var RpTwo = TargetRoomUser.Roleplayer;
        if (RpTwo == null)
        {
            return;
        }

        if (TargetRoomUser.GetClient().GetUser().Id == session.GetUser().Id)
        {
            return;
        }

        if (RpTwo.Dead || RpTwo.SendPrison)
        {
            return;
        }

        if (Rp.Money < NumberMoney)
        {
            return;
        }

        if (!(Math.Abs(TargetRoomUser.X - UserRoom.X) >= 2 || Math.Abs(TargetRoomUser.Y - UserRoom.Y) >= 2))
        {
            Rp.Money -= NumberMoney;
            RpTwo.Money += NumberMoney;

            Rp.SendUpdate();
            RpTwo.SendUpdate();

            TargetRoomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.receive", TargetRoomUser.GetClient().Langue), NumberMoney, UserRoom.GetUsername()));

            session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.send", session.Langue), NumberMoney, TargetRoomUser.GetUsername()));
            UserRoom.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.send.chat", session.Langue), TargetRoomUser.GetUsername()), 0, true);
        }
    }
}
