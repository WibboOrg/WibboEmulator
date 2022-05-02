using Butterfly.Game.Clients;
using Butterfly.Game.Roleplay.Player;
using System;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class GiveMoney : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 3)
            {
                return;
            }

            if (!Room.IsRoleplay)
            {
                return;
            }

            RolePlayer Rp = UserRoom.Roleplayer;
            if (Rp == null)
            {
                return;
            }

            if (Rp.Dead || Rp.SendPrison)
            {
                return;
            }

            RoomUser TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(Params[1].ToString());

            if (TargetRoomUser == null || TargetRoomUser.GetClient() == null || TargetRoomUser.GetClient().GetUser() == null)
            {
                return;
            }

            if (!int.TryParse(Params[2].ToString(), out int NumberMoney))
            {
                return;
            }

            if (NumberMoney <= 0)
            {
                return;
            }

            RolePlayer RpTwo = TargetRoomUser.Roleplayer;
            if (RpTwo == null)
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetUser().Id == Session.GetUser().Id)
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

            if (!((Math.Abs((TargetRoomUser.X - UserRoom.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - UserRoom.Y)) >= 2)))
            {
                Rp.Money -= NumberMoney;
                RpTwo.Money += NumberMoney;

                Rp.SendUpdate();
                RpTwo.SendUpdate();

                TargetRoomUser.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.receive", TargetRoomUser.GetClient().Langue), NumberMoney, UserRoom.GetUsername()));

                Session.SendWhisper(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.send", Session.Langue), NumberMoney, TargetRoomUser.GetUsername()));
                UserRoom.OnChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.givemoney.send.chat", Session.Langue), TargetRoomUser.GetUsername()), 0, true);
            }
        }
    }
}
