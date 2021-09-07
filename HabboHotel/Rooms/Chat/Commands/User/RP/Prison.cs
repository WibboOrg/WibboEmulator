﻿using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Roleplay.Player;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Prison : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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

            RoomUser TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(Params[1].ToString());
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
            }
            {
                UserRoom.OnChat("*Tente d'arrêter " + TargetRoomUser.GetUsername() + "*");
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.prisonnotallowed", Session.Langue));
                return;
            }
                TargetRoomUser.Freeze = true;
                TargetRoomUser.FreezeEndCounter = 0;
                TargetRoomUser.IsSit = true;
                TargetRoomUser.UpdateNeeded = true;

                RpTwo.SendPrison = true;

            //UserRoom.ApplyEffect(737, true);

            if (UserRoom.FreezeEndCounter <= 2)
            {
                UserRoom.Freeze = true;
                UserRoom.FreezeEndCounter = 2;
            }
        }
    }
}