using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Roleplay.Player;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Prison : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (!Room.IsRoleplay || !Room.Pvp)
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

            RoomUser TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(Params[1].ToString());            if (TargetRoomUser == null)
            {
                return;
            }

            RolePlayer RpTwo = TargetRoomUser.Roleplayer;
            if (RpTwo == null)
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (RpTwo.Dead || RpTwo.SendPrison)
            {
                return;
            }

            if (RpTwo.SendPrison)
            {
                return;
            }            if (Math.Floor((double)(RpTwo.Health / (double)RpTwo.HealthMax) * 100) > 75)
            {
                UserRoom.OnChat("*Tente d'arrêter " + TargetRoomUser.GetUsername() + "*");
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.prisonnotallowed", Session.Langue));
                return;
            }            if (!((Math.Abs((TargetRoomUser.X - UserRoom.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - UserRoom.Y)) >= 2)))            {                UserRoom.OnChat("*Arrête et envoie en prison " + TargetRoomUser.GetUsername() + "*");                TargetRoomUser.ApplyEffect(729, true);                TargetRoomUser.RotBody = 2;                TargetRoomUser.RotHead = 2;                TargetRoomUser.SetStatus("sit", "0.5");
                TargetRoomUser.Freeze = true;
                TargetRoomUser.FreezeEndCounter = 0;
                TargetRoomUser.IsSit = true;
                TargetRoomUser.UpdateNeeded = true;

                RpTwo.SendPrison = true;                RpTwo.PrisonTimer = 10 * 2;            }

            //UserRoom.ApplyEffect(737, true);            //UserRoom.TimerResetEffect = 2;

            if (UserRoom.FreezeEndCounter <= 2)
            {
                UserRoom.Freeze = true;
                UserRoom.FreezeEndCounter = 2;
            }
        }
    }
}
