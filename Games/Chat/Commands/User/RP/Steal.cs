namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Steal : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 3)
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

        var TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(Params[1].ToString());

        if (TargetRoomUser == null || TargetRoomUser.GetClient() == null || TargetRoomUser.GetClient().GetUser() == null)
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

        var NumberMoney = (int)Math.Floor((double)((double)Rp.Money / 100) * 15);

        if (Rp.Money < NumberMoney)
        {
            return;
        }

        if (!((Math.Abs(TargetRoomUser.X - UserRoom.X) >= 2) || (Math.Abs(TargetRoomUser.Y - UserRoom.Y) >= 2)))
        {
            Rp.Money += NumberMoney;
            RpTwo.Money -= NumberMoney;

            Rp.SendUpdate();
            RpTwo.SendUpdate();

            TargetRoomUser.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.vole.receive", TargetRoomUser.GetClient().Langue), NumberMoney, UserRoom.GetUsername()));

            session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.vole.send", session.Langue), NumberMoney, TargetRoomUser.GetUsername()));
            UserRoom.OnChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.vole.send.chat", session.Langue), TargetRoomUser.GetUsername()), 0, true);
        }
    }
}
